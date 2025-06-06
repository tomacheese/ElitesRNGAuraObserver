# ConfigData.cs レビュー

## 概要
アプリケーション設定データを格納するデータクラス。JSON形式でシリアライズされ、Discord Webhook URL、トースト通知設定、ログディレクトリ、Auras.jsonディレクトリの設定を管理している。

## レビュー結果

### 🟢 良い点

1. **データ検証の実装**
   - Discord Webhook URLの基本的な形式検証
   - プロパティセッターでの入力値チェック

2. **便利なユーティリティメソッド**
   - `Clone()`メソッドによる安全なオブジェクトコピー
   - `AreEqual()`メソッドによるリフレクションベースの比較機能

3. **適切なデフォルト値設定**
   - 各プロパティに実用的なデフォルト値を設定
   - AppConstantsからの定数参照によるメンテナンス性向上

4. **JSONシリアライゼーションの明示的制御**
   - `JsonPropertyName`属性による明確なプロパティマッピング

### 🟡 改善推奨事項

1. **JSONライブラリの混在**
   ```csharp
   // ConfigDataではSystem.Text.Json、AppConfigではNewtonsoft.Jsonを使用
   using System.Text.Json.Serialization; // ConfigData
   // vs
   JsonConvert.SerializeObject(config, Formatting.Indented); // AppConfig
   ```
   **推奨改善策：** どちらかのライブラリに統一

2. **URL検証の不十分性**
   ```csharp
   // 現在の実装：基本的なプロトコルチェックのみ
   private static bool IsValidUrl(string url)
   {
       return url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
              url.StartsWith("https://", StringComparison.OrdinalIgnoreCase);
   }
   ```
   **推奨改善策：**
   ```csharp
   private static bool IsValidDiscordWebhookUrl(string url)
   {
       if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
           return false;
           
       return uri.Scheme == "https" &&
              uri.Host.EndsWith("discord.com", StringComparison.OrdinalIgnoreCase) &&
              uri.AbsolutePath.StartsWith("/api/webhooks/", StringComparison.OrdinalIgnoreCase);
   }
   ```

3. **Clone()メソッドの手動実装リスク**
   ```csharp
   // 現在の実装：プロパティ追加時に更新が必要
   public ConfigData Clone()
   {
       return new ConfigData
       {
           DiscordWebhookUrl = DiscordWebhookUrl,
           ToastNotification = ToastNotification,
           LogDir = LogDir,
           AurasJsonDir = AurasJsonDir,
       };
   }
   ```
   **推奨改善策：**
   ```csharp
   public ConfigData Clone()
   {
       var json = JsonSerializer.Serialize(this);
       return JsonSerializer.Deserialize<ConfigData>(json) ?? new ConfigData();
   }
   ```

4. **パス系プロパティの検証不足**
   ```csharp
   // LogDirとAurasJsonDirで適切な検証が不足
   [JsonPropertyName("logDir")]
   public string LogDir { get; set; } = AppConstants.VRChatLogDirectory;
   ```

### 🔴 重要な問題

1. **機密情報の平文保存**
   ```csharp
   // Discord Webhook URLが平文で保存される重大なセキュリティリスク
   [JsonPropertyName("discordWebhookUrl")]
   public string DiscordWebhookUrl { get; set; }
   ```
   **推奨改善策：**
   ```csharp
   private string _encryptedWebhookUrl = string.Empty;

   [JsonPropertyName("encryptedWebhookUrl")]
   public string EncryptedWebhookUrl 
   { 
       get => _encryptedWebhookUrl; 
       set => _encryptedWebhookUrl = value; 
   }

   [JsonIgnore]
   public string DiscordWebhookUrl
   {
       get => string.IsNullOrEmpty(_encryptedWebhookUrl) ? 
              string.Empty : 
              DecryptWebhookUrl(_encryptedWebhookUrl);
       set
       {
           if (!string.IsNullOrEmpty(value) && !IsValidDiscordWebhookUrl(value))
               throw new ArgumentException("Invalid Discord Webhook URL.");
           _encryptedWebhookUrl = string.IsNullOrEmpty(value) ? 
                                  string.Empty : 
                                  EncryptWebhookUrl(value);
       }
   }
   ```

2. **設定値の包括的検証不足**
   - パスプロパティの存在確認なし
   - 設定値の相互依存関係チェックなし
   - 設定値の範囲チェックなし

3. **リフレクションを使用したAreEqual()メソッドのパフォーマンス懸念**
   ```csharp
   // 毎回リフレクションを使用するため、頻繁な比較でパフォーマンス問題の可能性
   foreach (PropertyInfo prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
   ```

## データ検証とバリデーション

### 現在の検証状況
- **Discord Webhook URL**: 基本的なプロトコルチェックのみ
- **その他のプロパティ**: 検証なし

### 推奨検証強化
```csharp
// パス系プロパティの検証例
private string _logDir = AppConstants.VRChatLogDirectory;

[JsonPropertyName("logDir")]
public string LogDir
{
    get => _logDir;
    set
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Log directory cannot be empty.");
        
        if (!IsValidDirectoryPath(value))
            throw new ArgumentException($"Invalid directory path: {value}");
            
        _logDir = value;
    }
}

private static bool IsValidDirectoryPath(string path)
{
    try
    {
        return Path.IsPathFullyQualified(path) && 
               !path.Any(c => Path.GetInvalidPathChars().Contains(c));
    }
    catch
    {
        return false;
    }
}
```

## スレッドセーフティ

**現在の状況：** スレッドセーフティの考慮なし

**懸念事項：**
- プロパティの読み書きが非同期で実行される可能性
- Clone()やAreEqual()メソッドの並行実行時の安全性

**推奨改善策：**
- 必要に応じてConcurrentCollectionの使用
- 重要な操作でのロック機構の導入

## セキュリティ考慮事項

### 重要度：高
1. **機密情報の暗号化実装**
   - DPAPI (Data Protection API)の使用を推奨
   - マシン固有またはユーザー固有のキーによる暗号化

2. **ログ出力時の機密情報漏洩防止**
   - ToString()メソッドのオーバーライド
   - デバッグ時の機密情報マスキング

### セキュリティ実装例
```csharp
public override string ToString()
{
    return $"ConfigData {{ " +
           $"DiscordWebhookUrl=[MASKED], " +
           $"ToastNotification={ToastNotification}, " +
           $"LogDir={LogDir}, " +
           $"AurasJsonDir={AurasJsonDir} }}";
}
```

## 総合評価

**評価: B-**

基本的なデータ管理機能は適切に実装されているが、セキュリティ面で重大な懸念がある。特に、Discord Webhook URLの平文保存は本格運用において重大なリスクとなる。データ検証の強化と機密情報の暗号化実装が急務。

### 優先改善項目
1. Discord Webhook URLの暗号化実装
2. 包括的なデータ検証機能の追加
3. JSONライブラリの統一
4. パフォーマンスを考慮したClone/比較メソッドの改善
5. セキュリティを考慮したログ出力機能の実装
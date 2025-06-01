# ElitesRNGAuraObserver/Core/Config/ConfigData.cs レビュー

## 概要

`ConfigData.cs`はアプリケーションの設定データを表すクラスです。Discord Webhook URL、トースト通知の有効/無効、ログディレクトリ、Auras.jsonのパスなどの設定を保持し、JSONでシリアライズ/デシリアライズされるように設計されています。

## 良い点

- プロパティに適切なバリデーションが実装されている（特にWebhook URL）
- JSON.NETとの連携のために適切な属性が使用されている
- XMLドキュメントコメントが適切に記述されており、プロパティの目的が明確
- オブジェクトの比較やクローン作成などの便利なユーティリティメソッドが提供されている

## 改善点

### 1. インターフェースの導入

```csharp
// 現状
internal class ConfigData

// 提案
// テスト容易性を向上させるためのインターフェース導入
public interface IConfigData
{
    string DiscordWebhookUrl { get; set; }
    bool ToastNotification { get; set; }
    string LogDir { get; set; }
    string AurasJsonDir { get; set; }

    IConfigData Clone();
}

internal class ConfigData : IConfigData
```

### 2. URL検証の強化

```csharp
// 現状
private static bool IsValidUrl(string url)
{
    return url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
           url.StartsWith("https://", StringComparison.OrdinalIgnoreCase);
}

// 提案
// より堅牢なURLバリデーション
private static bool IsValidUrl(string url)
{
    if (string.IsNullOrEmpty(url))
        return false;

    // 基本的なプロトコル検証
    if (!url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) &&
        !url.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
        return false;

    // Discord Webhook URLの形式検証
    return Uri.TryCreate(url, UriKind.Absolute, out var uri) &&
           (uri.Host.Equals("discord.com", StringComparison.OrdinalIgnoreCase) ||
            uri.Host.EndsWith(".discord.com", StringComparison.OrdinalIgnoreCase)) &&
           uri.AbsolutePath.Contains("/api/webhooks/");
}
```

### 3. ディレクトリパスの検証

```csharp
// 現状
// ディレクトリパスのバリデーションがない

// 提案
// ディレクトリパスのバリデーションとプロパティセッターの改善
private string _logDir = AppConstants.VRChatLogDirectory;

[JsonPropertyName("logDir")]
public string LogDir
{
    get => _logDir;
    set
    {
        if (string.IsNullOrEmpty(value))
            throw new ArgumentException("Log directory path cannot be empty", nameof(value));

        if (!Directory.Exists(value))
            throw new DirectoryNotFoundException($"Log directory does not exist: {value}");

        _logDir = value;
    }
}

private string _aurasJsonDir = AppConstants.ApplicationConfigDirectory;

[JsonPropertyName("aurasJsonDir")]
public string AurasJsonDir
{
    get => _aurasJsonDir;
    set
    {
        if (string.IsNullOrEmpty(value))
            throw new ArgumentException("Auras.json directory path cannot be empty", nameof(value));

        // ディレクトリが存在しない場合は作成を試みる
        if (!Directory.Exists(value))
        {
            try
            {
                Directory.CreateDirectory(value);
            }
            catch (Exception ex)
            {
                throw new DirectoryNotFoundException($"Failed to create Auras.json directory: {value}", ex);
            }
        }

        _aurasJsonDir = value;
    }
}
```

### 4. イベント通知の追加

```csharp
// 現状
// プロパティ変更通知がない

// 提案
// INotifyPropertyChangedインターフェースの実装
using System.ComponentModel;
using System.Runtime.CompilerServices;

internal class ConfigData : IConfigData, INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private string _discordWebhookUrl = string.Empty;

    [JsonPropertyName("discordWebhookUrl")]
    public string DiscordWebhookUrl
    {
        get => _discordWebhookUrl;
        set
        {
            if (!string.IsNullOrEmpty(value) && !IsValidUrl(value))
            {
                throw new ArgumentException("DiscordWebhookUrl must be a valid Discord webhook URL.");
            }

            if (_discordWebhookUrl != value)
            {
                _discordWebhookUrl = value;
                OnPropertyChanged();
            }
        }
    }

    // 他のプロパティも同様に実装
}
```

### 5. クローンと比較のリファクタリング

```csharp
// 現状
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

// 提案
// 汎用的なディープコピーメソッドの実装
public IConfigData Clone()
{
    return new ConfigData
    {
        DiscordWebhookUrl = DiscordWebhookUrl,
        ToastNotification = ToastNotification,
        LogDir = LogDir,
        AurasJsonDir = AurasJsonDir,
    };
}

// リフレクションを使ったクローン（より汎用的な方法）
public static T DeepCopy<T>(T source) where T : new()
{
    if (source == null) return default!;

    var type = typeof(T);
    var result = new T();

    foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
    {
        if (prop.CanWrite)
        {
            var value = prop.GetValue(source);
            prop.SetValue(result, value);
        }
    }

    return result;
}
```

## セキュリティ上の懸念

1. **Discord Webhook URL** - Webhook URLは機密情報であり、適切に保護されるべきです。ファイルへの保存時に暗号化を検討すべきです。
2. **ディレクトリアクセス** - ユーザーが指定したディレクトリにアクセスするため、適切な権限チェックが必要です。

## パフォーマンス上の懸念

1. **リフレクションの使用** - `AreEqual`メソッドはリフレクションを使用しており、頻繁に呼び出される場合はパフォーマンスに影響する可能性があります。キャッシュを利用するなどの最適化を検討すべきです。

## 命名規則

命名規則は適切に守られており、C#の標準的な命名規則（PascalCase、camelCase）に従っています。

## まとめ

全体的に適切に設計された設定クラスですが、インターフェースの導入、URL検証の強化、ディレクトリパスの検証、プロパティ変更通知の実装、およびクローンと比較のリファクタリングにより、コードの保守性、テスト容易性、および堅牢性を向上させることができます。特に、URLとディレクトリパスの検証は、アプリケーションの安定性と安全性を高めるために重要な改善点です。

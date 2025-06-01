# ElitesRNGAuraObserver/Core/Config/AppConfig.cs レビュー

## 概要

`AppConfig.cs`はアプリケーションの設定を管理するためのシングルトンスタイルのクラスです。設定の読み込み、保存、再読み込み、設定ディレクトリのパス管理などの機能を提供します。

## 良い点

- 設定の読み込み・保存を一元管理するクラスとして機能している
- 設定変更時のイベント通知の仕組みが実装されている
- 設定ファイルのパスをカスタマイズできる柔軟性がある
- 設定ファイルが存在しない場合のデフォルト設定生成がある

## 改善点

### 1. シングルトンパターンの改善

```csharp
// 現状
private static ConfigData _instance = new();
private static readonly Lock _lock = new();
private static bool _isLoaded = false;

public static ConfigData Instance
{
    get
    {
        // configファイルが存在している状態で一度読み込まれた場合、2回目以降は再読み込みしない
        if (File.Exists(Path.Combine(_configDir, ConfigFileName)) && !_isLoaded)
        {
            _instance = Load();
            _isLoaded = true;
        }

        return _instance!;
    }
}

// 提案
// スレッドセーフなLazy<T>を使用したシングルトンパターン
private static readonly Lazy<ConfigData> _lazyInstance = new(() =>
{
    var configPath = Path.Combine(_configDir, ConfigFileName);
    return File.Exists(configPath) ? Load() : new ConfigData();
}, LazyThreadSafetyMode.ExecutionAndPublication);

public static ConfigData Instance => _lazyInstance.Value;
```

### 2. 依存性の注入対応

```csharp
// 現状
// 静的クラスとして実装されており、依存性の注入ができない

// 提案
// インターフェースとサービス実装の分離
public interface IAppConfigService
{
    ConfigData GetConfig();
    void SaveConfig(ConfigData config);
    void ReloadConfig();
    string GetConfigDirectoryPath();
    void SetConfigDirectoryPath(string path);
    event Action<ConfigData> ConfigReloaded;
}

public class AppConfigService : IAppConfigService
{
    private readonly string _pathFileName;
    private readonly string _configFileName;
    private string _configDir;
    private ConfigData _currentConfig;
    private readonly object _lock = new();

    public event Action<ConfigData>? ConfigReloaded;

    public AppConfigService(string pathFileName = "config.path", string configFileName = "config.json")
    {
        _pathFileName = pathFileName;
        _configFileName = configFileName;
        _configDir = GetConfigDirectoryPath();
        _currentConfig = LoadConfig();
    }

    public ConfigData GetConfig() => _currentConfig;

    // 他のメソッド実装...
}

// 静的ファサードの提供（既存コードとの互換性のため）
internal static class AppConfig
{
    private static readonly IAppConfigService _service = new AppConfigService();

    public static event Action<ConfigData>? ConfigReloaded
    {
        add => _service.ConfigReloaded += value;
        remove => _service.ConfigReloaded -= value;
    }

    public static ConfigData Instance => _service.GetConfig();

    public static void Reload() => _service.ReloadConfig();

    public static void Save() => _service.SaveConfig(Instance);

    // 他のメソッド...
}
```

### 3. 例外処理の強化

```csharp
// 現状
// 例外処理が限定的

// 提案
// より堅牢な例外処理の追加
private static ConfigData Load()
{
    var configFilePath = Path.Combine(_configDir, ConfigFileName);

    try
    {
        if (!File.Exists(configFilePath))
        {
            var defaultConfig = new ConfigData();
            Save(defaultConfig);
            return defaultConfig;
        }

        var json = File.ReadAllText(configFilePath);
        var config = JsonConvert.DeserializeObject<ConfigData>(json);

        if (config == null)
        {
            Console.WriteLine($"Failed to deserialize config from {configFilePath}, using default config");
            return new ConfigData();
        }

        return config;
    }
    catch (IOException ex)
    {
        Console.WriteLine($"IO error reading config file: {ex.Message}");
        return new ConfigData();
    }
    catch (JsonException ex)
    {
        Console.WriteLine($"JSON parsing error in config file: {ex.Message}");
        // 破損した設定ファイルのバックアップ
        try
        {
            var backupPath = configFilePath + ".bak";
            File.Copy(configFilePath, backupPath, true);
            Console.WriteLine($"Corrupted config file backed up to {backupPath}");
        }
        catch (Exception backupEx)
        {
            Console.WriteLine($"Failed to backup corrupted config: {backupEx.Message}");
        }

        return new ConfigData();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Unexpected error loading config: {ex.Message}");
        return new ConfigData();
    }
}
```

### 4. ロギングの強化

```csharp
// 現状
// コンソール出力がない

// 提案
// 構造化ログの使用
private static readonly ILogger _logger = LoggerFactory.Create(builder =>
    builder.AddConsole()).CreateLogger(typeof(AppConfig));

public static void Save()
{
    _logger.LogInformation("Saving application configuration");
    if (_instance != null)
    {
        Save(_instance);
    }
    else
    {
        _logger.LogWarning("Configuration instance is null, nothing to save");
    }
}

private static void Save(ConfigData config)
{
    try
    {
        var json = JsonConvert.SerializeObject(config, Formatting.Indented);
        Directory.CreateDirectory(_configDir);
        File.WriteAllText(Path.Combine(_configDir, ConfigFileName), json);
        _logger.LogInformation("Configuration saved successfully to {ConfigPath}", Path.Combine(_configDir, ConfigFileName));
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Failed to save configuration to {ConfigPath}", Path.Combine(_configDir, ConfigFileName));
        throw;
    }
}
```

### 5. 設定の自動検証

```csharp
// 現状
// 設定ファイルからロードした設定のバリデーションがない

// 提案
// 設定の自動検証機能
private static ConfigData ValidateConfig(ConfigData config)
{
    var isValid = true;

    // ログディレクトリの検証
    if (!string.IsNullOrEmpty(config.LogDir) && !Directory.Exists(config.LogDir))
    {
        _logger.LogWarning("Log directory {LogDir} does not exist, using default", config.LogDir);
        config.LogDir = AppConstants.VRChatLogDirectory;
        isValid = false;
    }

    // Auras.jsonディレクトリの検証
    if (!string.IsNullOrEmpty(config.AurasJsonDir) && !Directory.Exists(config.AurasJsonDir))
    {
        try
        {
            Directory.CreateDirectory(config.AurasJsonDir);
            _logger.LogInformation("Created Auras.json directory: {AurasJsonDir}", config.AurasJsonDir);
        }
        catch
        {
            _logger.LogWarning("Failed to create Auras.json directory {AurasJsonDir}, using default", config.AurasJsonDir);
            config.AurasJsonDir = AppConstants.ApplicationConfigDirectory;
            isValid = false;
        }
    }

    // バリデーションが失敗した場合は設定を保存
    if (!isValid)
    {
        Save(config);
    }

    return config;
}

// Load()メソッド内で使用
var config = JsonConvert.DeserializeObject<ConfigData>(json) ?? new ConfigData();
return ValidateConfig(config);
```

## セキュリティ上の懸念

1. **設定ファイルの保護** - 設定ファイルに機密情報（Discord Webhook URL）が平文で保存されており、セキュリティリスクがあります。機密情報の暗号化を検討すべきです。
2. **ファイルシステム操作** - ファイルの読み書きが適切な例外処理なしで行われており、セキュリティ上のリスクがあります。

## パフォーマンス上の懸念

1. **設定の頻繁な読み込み** - 現在の実装では、設定ファイルの存在チェックが`Instance`プロパティのアクセスごとに行われる可能性があります。キャッシュを効果的に活用するべきです。

## 命名規則

命名規則は適切に守られており、C#の標準的な命名規則（PascalCase、camelCase）に従っています。

## まとめ

全体的に機能的な設定管理クラスですが、シングルトンパターンの改善、依存性の注入への対応、例外処理の強化、構造化ログの導入、および設定の自動検証により、コードの保守性、テスト容易性、および堅牢性を向上させることができます。特に、依存性の注入とより堅牢な例外処理は、アプリケーションの安定性とテスト容易性を高めるために重要な改善点です。

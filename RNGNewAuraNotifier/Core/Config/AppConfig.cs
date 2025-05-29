using Newtonsoft.Json;

namespace RNGNewAuraNotifier.Core.Config;

/// <summary>
/// アプリケーションの設定を管理するクラス
/// </summary>
internal class AppConfig
{
    private static string _configPath = GetConfigDirectoryPath();
    private static ConfigData _instance = new();
    private static readonly Lock _lock = new();
    private static bool _isLoaded = false;

    /// <summary>
    /// 設定が再読み込みされたときに発生するイベント
    /// </summary>
    public static event Action<ConfigData>? ConfigReloaded;

    /// <summary>
    /// config.jsonの保存先ディレクトリを取得します。
    /// </summary>
    /// <returns>config.jsonの保存先パス</returns>
    public static string GetConfigDirectoryPath()
    {
        var filePath = Path.Combine(AppConstants.ApplicationConfigDirectory, "config.path");

        if (File.Exists(filePath))
        {
            return File.ReadAllText(filePath).Trim();
        }
        else
        {
            // デフォルトの設定ディレクトリを返す
            return AppConstants.ApplicationConfigDirectory;
        }
    }

    /// <summary>
    /// 設定ディレクトリのパスを保存します。
    /// </summary>
    /// <param name="configPath">config.jsonのパス</param>
    public static void SaveConfigDirectoryPath(string configPath)
    {
        var newConfigPath = Path.Combine(configPath);
        var configDir = AppConstants.ApplicationConfigDirectory;
        var filePath = Path.Combine(configDir, "config.path");
        Directory.CreateDirectory(configDir);
        File.WriteAllText(filePath, newConfigPath);
        _configPath = newConfigPath;
    }

    /// <summary>
    /// 設定データのインスタンスを取得します。
    /// </summary>
    public static ConfigData Instance
    {
        get
        {
            // configファイルが存在している状態で一度読み込まれた場合、2回目以降は再読み込みしない
            if (File.Exists(_configPath) && !_isLoaded)
            {
                _instance = Load();
                _isLoaded = true;
            }

            return _instance!;
        }
    }

    /// <summary>
    /// 設定の再読み込み
    /// </summary>
    public static void Reload()
    {
        lock (_lock)
        {
            _instance = Load();
            ConfigReloaded?.Invoke(_instance!);
        }
    }

    /// <summary>
    /// 設定の保存
    /// </summary>
    public static void Save()
    {
        if (_instance != null)
        {
            Save(_instance);
        }
    }

    /// <summary>
    /// 設定の保存
    /// </summary>
    /// <param name="config">コンフィグ情報</param>
    private static void Save(ConfigData config)
    {
        var json = JsonConvert.SerializeObject(config, Formatting.Indented);
        Directory.CreateDirectory(Path.GetDirectoryName(_configPath)!);
        File.WriteAllText(_configPath, json);
    }

    /// <summary>
    /// 設定の読み込み
    /// </summary>
    private static ConfigData Load()
    {
        if (!File.Exists(_configPath))
        {
            var defaultConfig = new ConfigData();
            Save(defaultConfig);
            return defaultConfig;
        }

        var json = File.ReadAllText(_configPath);
        return JsonConvert.DeserializeObject<ConfigData>(json)
               ?? new ConfigData();
    }
}

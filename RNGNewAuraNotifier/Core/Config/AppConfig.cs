using Newtonsoft.Json;

namespace RNGNewAuraNotifier.Core.Config;

/// <summary>
/// アプリケーションの設定を管理するクラス
/// </summary>
internal class AppConfig
{
    private static string _configDir = GetConfigDirectoryPath();
    private const string PathFileName = "config.path";
    private const string ConfigFileName = "config.json";
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
        var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, PathFileName);

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
        var appDir = AppDomain.CurrentDomain.BaseDirectory;
        var filePath = Path.Combine(appDir, PathFileName);

        if (string.Equals(configPath, AppConstants.ApplicationConfigDirectory, StringComparison.OrdinalIgnoreCase))
        {
            // 規定値の場合は config.path を削除
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
        else
        {
            // 規定値以外の場合は config.path を作成
            File.WriteAllText(filePath, configPath);
        }

        _configDir = configPath;
    }

    /// <summary>
    /// 設定データのインスタンスを取得します。
    /// </summary>
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
        Directory.CreateDirectory(_configDir);
        File.WriteAllText(Path.Combine(_configDir, ConfigFileName), json);
    }

    /// <summary>
    /// 設定の読み込み
    /// </summary>
    private static ConfigData Load()
    {
        var configFilePath = Path.Combine(_configDir, ConfigFileName);
        if (!File.Exists(configFilePath))
        {
            var defaultConfig = new ConfigData();
            Save(defaultConfig);
            return defaultConfig;
        }

        var json = File.ReadAllText(configFilePath);
        return JsonConvert.DeserializeObject<ConfigData>(json)
               ?? new ConfigData();
    }
}

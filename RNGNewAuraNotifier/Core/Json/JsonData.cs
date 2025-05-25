using System.Text;
using Newtonsoft.Json;
using RNGNewAuraNotifier.Properties;

namespace RNGNewAuraNotifier.Core.Json;

/// <summary>
/// JSONデータを格納するクラス
/// </summary>

internal class JsonData
{

    /// <summary>
    /// JSONのバージョン情報
    /// </summary>
    [JsonProperty("Version")]
    private readonly string _version = string.Empty;

    /// <summary>
    /// Auraの一覧
    /// </summary>
    [JsonProperty("Auras")]
    private readonly Aura.Aura[] _auras = [];

    /// <summary>
    /// 最新のJSONデータを取得する非同期メソッド
    /// </summary>
    public static async Task GetLatestJsonDataAsync()
    {
        var jsonUpdate = new JsonUpdateService(AppConstants.GitHubRepoOwner, AppConstants.GitHubRepoName);

        try
        {
            await jsonUpdate.FetchMasterJsonAsync().ConfigureAwait(true);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching latest JSON data: {ex.Message}");
        }
    }

    /// <summary>
    /// JSONファイルの内容を取得する
    /// </summary>
    /// <returns>JSONファイルの内容</returns>
    public static JsonData GetJsonData()
    {
        // JSONデータを文字列に変換
        var jsonContent = Encoding.UTF8.GetString(Resources.Auras);

        // Jsonファイルの保存先
        var jsonDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "RNGNewAuraNotifier", "Aura.json");
        if (File.Exists(jsonDir))
        {
            jsonContent = File.ReadAllText(jsonDir);
        }
        JsonData jsonData = JsonConvert.DeserializeObject<JsonData>(jsonContent) ?? new JsonData();
        return jsonData;
    }

    /// <summary>
    /// JSONのバージョン情報を取得する
    /// </summary>
    /// <returns>JSONのバージョン情報</returns>
    public static string GetVersion()
    {
        try
        {
            JsonData jsonData = GetJsonData();
            return jsonData._version;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error Could not get JSON version: {ex.Message}");
            return string.Empty;
        }
    }

    /// <summary>
    /// Auraの情報を取得する
    /// </summary>
    /// <returns>Auraの情報</returns>
    public static Aura.Aura[] GetAuras()
    {
        try
        {
            Aura.Aura[] auras = GetJsonData()._auras ?? [];
            return auras;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deserializing Aura data: {ex.Message}");
            return [];
        }
    }
}

using System.Text;
using ElitesRNGAuraObserver.Core.Config;
using ElitesRNGAuraObserver.Properties;
using Newtonsoft.Json;

namespace ElitesRNGAuraObserver.Core.Json;

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
    /// JSONファイルの内容を取得する
    /// </summary>
    /// <returns>JSONファイルの内容</returns>
    public static JsonData GetJsonData()
    {
        ConfigData configData = AppConfig.Instance;
        // Jsonファイルの保存先
        var jsonFilePath = Path.Combine(configData.AurasJsonDir, "Auras.json");
        string? jsonContent;

        // 1. 保存先JSONファイルが存在する場合はそれを読む
        if (File.Exists(jsonFilePath))
        {
            try
            {
                jsonContent = File.ReadAllText(jsonFilePath);
                JsonData? jsonData = JsonConvert.DeserializeObject<JsonData>(jsonContent) ?? new JsonData();
                return jsonData;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Could not deserialize local JSON data: {ex.Message}");
            }
        }

        // 保存先JSONファイルが存在しない場合、またはデシリアライズに失敗した場合はResourcesから読み込む
        jsonContent = Encoding.UTF8.GetString(Resources.Auras);
        JsonData? resourceJsonData = JsonConvert.DeserializeObject<JsonData>(jsonContent) ?? new JsonData();
        return resourceJsonData;
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

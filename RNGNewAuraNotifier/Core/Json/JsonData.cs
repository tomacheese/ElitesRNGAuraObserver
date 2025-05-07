using System.Text;
using Newtonsoft.Json;
using RNGNewAuraNotifier.Properties;

namespace RNGNewAuraNotifier.Core.Json;
internal class JsonData
{
    public string Version { get; set; } = string.Empty;
    public Aura.Aura[] Auras { get; set; } = Array.Empty<Aura.Aura>();

    public static JsonData GetJsonData()
    {
        try
        {
            // JSONデータを文字列に変換
            var jsonContent = Encoding.UTF8.GetString(Resources.Auras);
            JsonData jsonData = JsonConvert.DeserializeObject<JsonData>(jsonContent) ?? new JsonData();
            return jsonData;
        }
        catch
        {
            throw;
        }
    }

    /// <summary>
    /// JSONのバージョン情報を取得する
    /// </summary>
    /// <returns></returns>
    public static string GetVersion()
    {
        try
        {
            JsonData jsonData = GetJsonData();
            return jsonData.Version;
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
    /// <returns></returns>
    public static Aura.Aura[] GetAuras()
    {
        try
        {
            Aura.Aura[] auras = GetJsonData().Auras ?? Array.Empty<Aura.Aura>();
            return auras;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deserializing Aura data: {ex.Message}");
            return Array.Empty<Aura.Aura>();
        }
    }
}
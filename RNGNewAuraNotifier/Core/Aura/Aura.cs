using Newtonsoft.Json;
using RNGNewAuraNotifier.Properties;
using System.Text;

namespace RNGNewAuraNotifier.Core.Aura;
internal class Aura
{
    /// <summary>
    /// Aura の ID
    /// </summary>
    /// <example>60</example>
    public required string Id { get; set; }

    /// <summary>
    /// Aura の名前
    /// </summary>
    /// <example>Celebration</example>
    public required string? Name { get; set; }

    /// <summary>
    /// Aura を取得する
    /// </summary>
    /// <param name="auraId">Aura の ID</param>
    /// <returns>Aura のインスタンス</returns>
    public static Aura GetAura(string auraId)
    {
        var jsonContent = Encoding.UTF8.GetString(Resources.Auras);
        Dictionary<string, string> auras = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonContent) ?? [];

        var auraName = auras.TryGetValue(auraId, out var name) ? name : null;
        return new Aura
        {
            Id = auraId,
            Name = auraName
        };
    }
}

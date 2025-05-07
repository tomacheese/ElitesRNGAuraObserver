using System.Text;
using RNGNewAuraNotifier.Core.Json;
using RNGNewAuraNotifier.Properties;

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
    /// オーラの当選確率
    /// </summary>
    public required int Rarity { get; set; } = 0;

    /// <summary>
    /// オーラのティア
    /// </summary>
    public required int Tier { get; set; } = 0;

    /// <summary>
    /// オーラのサブテキスト
    /// </summary>
    public required string SubText { get; set; }


    /// <summary>
    /// Aura を取得する
    /// </summary>
    /// <param name="auraId">Aura の ID</param>
    /// <returns>Aura のインスタンス</returns>
    public static Aura GetAura(string auraId)
    {
        try
        {
            // JSONデータを文字列に変換
            var jsonContent = Encoding.UTF8.GetString(Resources.Auras);

            // JSONをAura[]にデシリアライズ
            Aura[] auras = JsonData.GetAuras();

            // 指定されたIDのAuraを検索
            Aura? auraInfo = auras.First(aura => aura.Id == auraId);

            return auraInfo;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deserializing Auras: {ex.Message}");
            return new Aura
            {
                Id = auraId,
                Name = "Unknown",
                Rarity = 0,
                Tier = 0,
                SubText = $"Aura#: {auraId}",
            };
        }
    }

    public string? GetNameText() => string.IsNullOrEmpty(SubText) ? Name : $"{Name} ({SubText})";

    public string GetRarityString() => Rarity != 0 ? $"1 in {Rarity:N0}" : "???";
}
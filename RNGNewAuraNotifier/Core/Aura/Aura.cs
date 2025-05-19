using System.Text;
using RNGNewAuraNotifier.Core.Json;
using RNGNewAuraNotifier.Properties;

namespace RNGNewAuraNotifier.Core.Aura;
internal class Aura(string id, string? name = null, int rarity = 0, int tier = 0, string subText = "")
{
    /// <summary>
    /// Aura の ID
    /// </summary>
    /// <example>60</example>
    public string Id { get; private set; } = id;

    /// <summary>
    /// Aura の名前
    /// </summary>
    /// <example>Celebration</example>
    public string? Name { get; private set; } = name;

    /// <summary>
    /// オーラの当選確率
    /// </summary>
    /// <example>1000000</example>
    public int Rarity { get; private set; } = rarity;

    /// <summary>
    /// オーラのティア
    /// </summary>
    /// <remarks>
    /// Rarityの高さによる区分け、ゲーム内の演出をもとに割り振っている。
    /// 
    /// Rarity: ～ 999 Tier:5
    /// Rarity: 1000 ～ 9999 Tier:4
    /// Rarity: 10000 ～ 99999 Tier:3
    /// Rarity: 100000 ～ 999999 Tier:2
    /// Rarity: 1000000 ～ 9999999 Tier:1
    /// SPECIAL枠のAura(特殊な入手条件のAuraのみ)はTier:0
    /// </remarks>
    /// <example>4</example>
    public int Tier { get; private set; } = tier;

    /// <summary>
    /// オーラのサブテキスト
    /// </summary>
    /// <example>VALENTINE’S EXCLUSIVE</example>
    public string SubText { get; private set; } = subText;

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

            return auras.FirstOrDefault(aura => aura.Id == auraId) ?? new Aura(auraId);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deserializing Auras({ex.GetType().Name}): {ex.Message}");
            return new Aura(auraId);
        }
    }

    /// <summary>
    /// オーラの名前を取得する
    /// </summary>
    /// <remarks>
    /// subTextがnullの場合、Nameをそのまま返す。
    /// subTextが存在する場合、NameとsubTextを括弧で囲んで結合する。
    /// </remarks>
    /// <example>
    /// "Event Horizon"
    /// "Cupid (VALENTINE’S EXCLUSIVE)"
    /// </example>
    /// <returns>通知に表示するオーラ名称</returns>
    public string? GetNameText() => string.IsNullOrEmpty(SubText) ? Name : $"{Name} ({SubText})";

    /// <summary>
    /// オーラのレアリティを取得する
    /// </summary>
    /// <remarks>
    /// Rarityが0では無い場合、"1 in"の後にレアリティの数値をカンマ区切りで表示。
    /// Rarityが0の場合、"???"を表示。
    /// </remarks>
    /// <example>
    /// "1 in 1,000,000"
    /// "???"
    /// </example>
    /// <returns>通知に表示するレアリティ</returns>
    public string GetRarityString() => Rarity != 0 ? $"1 in {Rarity:N0}" : "???";
}

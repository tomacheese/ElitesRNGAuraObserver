using RNGNewAuraNotifier.Core.Json;

namespace RNGNewAuraNotifier.Core.Aura;

/// <summary>
/// Auraの情報を表すレコード
/// </summary>
internal record Aura
{
    /// <summary>
    /// Aura の ID
    /// </summary>
    /// <example>60</example>
    public string Id { get; init; }

    /// <summary>
    /// Aura の名前
    /// </summary>
    /// <example>Celebration</example>
    public string? Name { get; init; }

    /// <summary>
    /// オーラの当選確率
    /// </summary>
    /// <example>1000000</example>
    public int Rarity { get; init; }

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
    public int Tier { get; init; }

    /// <summary>
    /// オーラのサブテキスト
    /// </summary>
    /// <example>VALENTINE’S EXCLUSIVE</example>
    public string SubText { get; init; } = "";

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="id">Aura の ID</param>
    /// <param name="name">Aura の名前</param>
    /// <param name="rarity">オーラの当選確率</param>
    /// <param name="tier">オーラのティア</param>
    /// <param name="subText">オーラのサブテキスト</param>
    public Aura(string id, string? name = null, int rarity = 0, int tier = 0, string subText = "")
    {
        Id = id;
        Name = name;
        Rarity = rarity;
        Tier = tier;
        SubText = subText;
    }

    /// <summary>
    /// Aura を取得する
    /// </summary>
    /// <param name="auraId">Aura の ID</param>
    /// <returns>Aura のインスタンス</returns>
    public static Aura GetAura(string auraId)
    {
        try
        {
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

    /// <summary>
    /// オーラの等価性を比較する
    /// </summary>
    /// <param name="other">比較対象のAura</param>
    /// <returns>等価であればtrue、そうでなければfalse</returns>
    public virtual bool Equals(Aura? other) => other != null && Id == other.Id;

    /// <summary>
    /// オーラのハッシュコードを取得する
    /// </summary>
    /// <returns>ハッシュコード</returns>
    public override int GetHashCode() => Id.GetHashCode();
}

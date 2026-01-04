using ElitesRNGAuraObserver.Core.Json;

namespace ElitesRNGAuraObserver.Core.Aura;

/// <summary>
/// Auraの情報を表すレコード
/// </summary>
internal record Aura
{
    /// <summary>
    /// Aura の ID
    /// </summary>
    /// <example>60</example>
    public int Id { get; init; }

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
    /// オーラのカテゴリ
    /// </summary>
    /// <remarks>
    /// JSONのカテゴリーには対応する AuraCategory 列挙体の値が入る。
    /// </remarks>
    /// <example>0</example>
    public int Category { get; init; }

    /// <summary>
    /// オーラのサブテキスト
    /// </summary>
    /// <example>VALENTINE’S EXCLUSIVE</example>
    public string SubText { get; init; } = string.Empty;

    /// <summary>
    /// Special枠のオーラかどうか
    /// </summary>
    public bool Special { get; init; } = false;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="id">Aura の ID</param>
    /// <param name="name">Aura の名前</param>
    /// <param name="rarity">オーラの当選確率</param>
    /// <param name="category">オーラのカテゴリ</param>
    /// <param name="subText">オーラのサブテキスト</param>
    /// <param name="special">Special枠のオーラかどうか</param>
    public Aura(int id, string? name = null, int rarity = 0, int category = 0, string subText = "", bool special = false)
    {
        Id = id;
        Name = name;
        Rarity = rarity;
        Category = category;
        SubText = subText;
        Special = special;
    }

    /// <summary>
    /// Aura を取得する
    /// </summary>
    /// <param name="auraId">Aura の ID</param>
    /// <returns>Aura のインスタンス</returns>
    public static Aura GetAura(int auraId)
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

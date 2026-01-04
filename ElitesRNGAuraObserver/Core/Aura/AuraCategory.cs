namespace ElitesRNGAuraObserver.Core.Aura;

/// <summary>
/// Auraのカテゴリを表すクラス
/// </summary>
internal class AuraCategory
{
    /// <summary>
    /// Auraのカテゴリ定義
    /// </summary>
    internal readonly struct AuraCategories
    {
        /// <summary>
        /// UnknownカテゴリのID
        /// </summary>
        /// <remarks>
        /// Aura情報が取得できない場合に設定されます。
        /// </remarks>
        public const int Unknown = 0;

        /// <summary>
        /// OrdinaryカテゴリのID
        /// </summary>
        public const int Ordinary = 1;

        /// <summary>
        /// RefinedカテゴリのID
        /// </summary>
        public const int Refined = 2;

        /// <summary>
        /// EnhancedカテゴリのID
        /// </summary>
        public const int Enhanced = 3;

        /// <summary>
        /// AdvancedカテゴリのID
        /// </summary>
        public const int Advanced = 4;

        /// <summary>
        /// FormidableカテゴリのID
        /// </summary>
        public const int Formidable = 5;

        /// <summary>
        /// CatastrophicカテゴリのID
        /// </summary>
        public const int Catastrophic = 6;

        /// <summary>
        /// AnnihilatoryカテゴリのID
        /// </summary>
        public const int Annihilatory = 7;

        /// <summary>
        /// ExclusiveカテゴリのID
        /// </summary>
        public const int Exclusive = 8;

        /// <summary>
        /// ValentinesカテゴリのID
        /// </summary>
        public const int Valentines = 9;

        /// <summary>
        /// HalloweenカテゴリのID
        /// </summary>
        public const int Halloween = 10;

        /// <summary>
        /// ChristmasカテゴリのID
        /// </summary>
        public const int Christmas = 11;
    }
}

namespace ElitesRNGAuraObserver.Updater.Core;

/// <summary>
/// GitHubのリリース情報
/// </summary>
/// <param name="tagName">タグ名</param>
/// <param name="assetUrl">アセット URL</param>
/// <param name="assetDigest">アセットのダイジェスト</param>
internal class ReleaseInfo(string tagName, string assetUrl, string assetDigest)
{
    /// <summary>
    /// リリースのタグ名
    /// </summary>
    public SemanticVersion Version { get; } = SemanticVersion.Parse(tagName.TrimStart('v'));

    /// <summary>
    /// アセットのURL
    /// </summary>
    public string AssetUrl { get; } = assetUrl;

    /// <summary>
    /// アセットのダイジェスト
    /// </summary>
    /// <example>sha256:617d6861ded8113abbdb00d0e2ae0318fec957d53a9c9b40f33ac9b800ae54b6</example>
    public string AssetDigest { get; } = assetDigest;
}

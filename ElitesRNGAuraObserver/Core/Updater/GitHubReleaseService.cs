using System.Text.Json.Nodes;

namespace ElitesRNGAuraObserver.Core.Updater;

/// <summary>
/// GitHubのリリース情報を取得するサービス
/// </summary>
internal class GitHubReleaseService : IDisposable
{
    private readonly HttpClient _http;
    private readonly string _owner;
    private readonly string _repo;

    /// <summary>
    /// GitHubのリポジトリ情報を指定してインスタンスを生成する
    /// </summary>
    /// <param name="owner">リポジトリオーナー</param>
    /// <param name="repo">リポジトリ名</param>
    public GitHubReleaseService(string owner, string repo)
    {
        _owner = owner;
        _repo = repo;
        _http = new HttpClient();
        var userAgent = $"{owner} {repo} ({AppConstants.AppVersionString})";
        _http.DefaultRequestHeaders.UserAgent.ParseAdd(userAgent);
    }

    /// <summary>
    /// 最新リリースの情報を取得する
    /// </summary>
    /// <param name="assetName">アセット名</param>
    /// <returns>リリース情報</returns>
    /// <exception cref="InvalidOperationException">アセットが見つからない場合</exception>
    public async Task<ReleaseInfo> GetLatestReleaseAsync(string assetName)
    {
        var url = new Uri($"https://api.github.com/repos/{_owner}/{_repo}/releases/latest");
        var json = await _http.GetStringAsync(url).ConfigureAwait(false);
        var obj = JsonNode.Parse(json);
        var tagName = obj?["tag_name"]?.ToString() ?? throw new InvalidOperationException("Failed to parse tag_name");
        var assetUrl = obj?["assets"]?.AsArray()
            .FirstOrDefault(x => x?["name"]?.ToString() == assetName)?["browser_download_url"]?.ToString();
        return string.IsNullOrEmpty(assetUrl)
            ? throw new InvalidOperationException($"Failed to find asset: {assetName}")
            : new ReleaseInfo(tagName, assetUrl);
    }

    /// <inheritdoc/>
    public void Dispose() => _http.Dispose();
}

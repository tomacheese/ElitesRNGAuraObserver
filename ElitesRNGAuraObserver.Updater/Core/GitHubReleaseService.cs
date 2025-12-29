using System.Text.Json;
using System.Text.Json.Nodes;

namespace ElitesRNGAuraObserver.Updater.Core;

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
        Console.WriteLine($"GET {url}");
        var json = await _http.GetStringAsync(url).ConfigureAwait(false);
        JsonNode? obj = JsonNode.Parse(json);
        var tagName = obj?["tag_name"]?.ToString() ?? throw new InvalidOperationException("Failed to parse tag_name");
        JsonNode? asset = obj?["assets"]?.AsArray()
            .FirstOrDefault(x => x?["name"]?.ToString() == assetName) ?? throw new InvalidOperationException($"Asset '{assetName}' not found in the latest release.");

        var assetUrl = asset["browser_download_url"]?.ToString();
        var assetDigest = asset["digest"]?.ToString();

        return string.IsNullOrEmpty(assetUrl) || string.IsNullOrEmpty(assetDigest)
            ? throw new InvalidOperationException($"Asset '{assetName}' does not have a valid URL or digest.")
            : new ReleaseInfo(tagName, assetUrl, assetDigest);
    }

    /// <summary>
    /// 指定したURLからファイルをダウンロードして一時ファイルに格納する
    /// </summary>
    /// <param name="url">URL</param>
    /// <returns>一時ファイルのパス</returns>
    public async Task<string> DownloadWithProgressAsync(string url)
    {
        var tmp = Path.GetTempFileName();
        var uri = new Uri(url);
        using HttpResponseMessage res = await _http.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);
        res.EnsureSuccessStatusCode();

        var total = res.Content.Headers.ContentLength ?? -1L;
        using Stream stream = await res.Content.ReadAsStreamAsync().ConfigureAwait(false);
        using FileStream fs = File.OpenWrite(tmp);

        var buffer = new byte[81920];
        long downloaded = 0;
        int read;
        while ((read = await stream.ReadAsync(buffer).ConfigureAwait(false)) > 0)
        {
            await fs.WriteAsync(buffer.AsMemory(0, read)).ConfigureAwait(false);
            downloaded += read;
            if (total > 0)
            {
                Console.Write($"\r{downloaded:#,0}/{total:#,0} bytes");
            }
        }

        Console.WriteLine();
        return tmp;
    }

    /// <inheritdoc/>
    public void Dispose() => _http.Dispose();
}

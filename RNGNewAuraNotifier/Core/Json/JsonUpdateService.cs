using System.Text;
using RNGNewAuraNotifier.Properties;

namespace RNGNewAuraNotifier.Core.Json;

/// <summary>
/// Aura.jsonの更新を管理するクラス
/// </summary>
internal class JsonUpdateService(string owner, string repo) : IDisposable
{
    private readonly HttpClient _http = new();
    private readonly string _owner = owner;
    private readonly string _repo = repo;

    /// <summary>
    /// masterブランチのAuras.jsonを取得し、最新のバージョンだったらローカルに保存する
    /// </summary>
    public async Task FetchMasterJsonAsync()
    {
        var url = new Uri($"https://raw.githubusercontent.com/{_owner}/{_repo}/master/{_repo}/Resources/Auras.json");
        var saveDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "RNGNewAuraNotifier", "Aura.json");

        // ディレクトリが存在しない場合は作成
        Directory.CreateDirectory(Path.GetDirectoryName(saveDir));

        using var client = new HttpClient();
        var jsonContent = await client.GetStringAsync(url).ConfigureAwait(true);

        if (CheckUpdateJsonData(jsonContent))
        {
            await File.WriteAllTextAsync(saveDir, jsonContent).ConfigureAwait(true);
            Console.WriteLine($"Json file saved. Path: {saveDir}");
        }
    }

    /// <summary>
    /// JSONデータの更新が必要かどうかを確認する
    /// </summary>
    /// <param name="fetchJsonContent">ダウンロードしたJSON文字列</param>
    /// <returns>true:アップデートする/false:アップデートしない</returns>
    private static bool CheckUpdateJsonData(string fetchJsonContent)
    {
        // Jsonファイルの保存先
        var jsonDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "RNGNewAuraNotifier", "Aura.json");
        var currentJsonContent = Encoding.UTF8.GetString(Resources.Auras);

        if (File.Exists(jsonDir))
        {
            currentJsonContent = File.ReadAllText(jsonDir);
        }

        return !string.Equals(fetchJsonContent, currentJsonContent, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// リソースを解放する
    /// </summary>
    public void Dispose() => _http.Dispose();
}

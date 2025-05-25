using System.Text;
using Newtonsoft.Json;
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
        var saveDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "RNGNewAuraNotifier");

        // ディレクトリが存在しない場合は作成
        if (!Directory.Exists(saveDir))
        {
            Directory.CreateDirectory(saveDir);
        }

        using var client = new HttpClient();
        var jsonContent = await client.GetStringAsync(url).ConfigureAwait(true);

        if (CheckUpdateJsonData(jsonContent))
        {
            await File.WriteAllTextAsync(Path.Combine(saveDir, "Aura.json"), jsonContent).ConfigureAwait(true);
            Console.WriteLine($"Json file saved.  Path: {saveDir}");
        }
    }

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

    public void Dispose() => _http.Dispose();
}

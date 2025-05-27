using Newtonsoft.Json.Linq;

namespace RNGNewAuraNotifier.Core.Json;

/// <summary>
/// Aura.jsonの更新を管理するクラス
/// </summary>
internal class JsonUpdateService(string owner, string repo)
{
    private readonly string _owner = owner;
    private readonly string _repo = repo;

    /// <summary>
    /// masterブランチのAuras.jsonを取得し、最新のバージョンだったらローカルに保存する
    /// </summary>
    /// <returns>masterブランチのAuras.jsonが取得され、最新バージョンであればローカルに保存されたときに完了するタスク</returns>
    public async Task FetchMasterJsonAsync()
    {
        var url = new Uri($"https://raw.githubusercontent.com/{_owner}/{_repo}/master/{_repo}/Resources/Auras.json");
        var saveDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "RNGNewAuraNotifier", "Aura.json");

        // ディレクトリが存在しない場合は作成
        var dir = Path.GetDirectoryName(saveDir);
        if (dir is not null)
        {
            Directory.CreateDirectory(dir);
        }

        using var client = new HttpClient();
        var jsonContent = await client.GetStringAsync(url).ConfigureAwait(false);

        // Versionフィールドを取得
        string? version = null;
        try
        {
            var jObject = JObject.Parse(jsonContent);
            version = jObject["Version"]?.ToString();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to get Version: {ex.Message}");
        }

        // JSONファイルの更新チェック
        if (version is null || CheckUpdateJsonData(version))
        {
            await File.WriteAllTextAsync(saveDir, jsonContent).ConfigureAwait(false);
            Console.WriteLine($"Json file saved. Path: {saveDir}");
        }
    }

    /// <summary>
    /// JSONデータの更新が必要かどうかを確認する
    /// </summary>
    /// <param name="fetchJsonVersion">ダウンロードしたJSON文字列</param>
    /// <returns>true:アップデートする/false:アップデートしない</returns>
    private static bool CheckUpdateJsonData(string fetchJsonVersion)
    {
        var currentJsonVersion = JsonData.GetVersion();

        return DateTime.TryParse(fetchJsonVersion, out DateTime fetchDate) &&
            DateTime.TryParse(currentJsonVersion, out DateTime currentDate) && fetchDate > currentDate;
    }
}

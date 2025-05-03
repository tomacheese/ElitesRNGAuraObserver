using System.Text.RegularExpressions;
using RNGNewAuraNotifier.Core.VRChat;

namespace RNGNewAuraNotifier.Core.Aura;
internal partial class NewAuraDetectionService
{
    /// <summary>
    /// Aura取得時のイベント
    /// </summary>
    public event Action<Aura, bool> OnDetected = delegate { };

    /// <summary>
    /// Aura取得時のログパターン
    /// </summary>
    /// <example>2025.04.16 18:07:07 Debug      -  [<color=green>Elite's RNG Land</color>] Successfully legitimized Aura #60.</example>
    [GeneratedRegex(@"(?<datetime>[0-9]{4}\.[0-9]{2}.[0-9]{2} [0-9]{2}:[0-9]{2}:[0-9]{2}) (?<Level>.[A-z]+) *- *\[<color=green>Elite's RNG Land</color>\] Successfully legitimized Aura #(?<AuraId>[0-9]{2})\.")]
    private static partial Regex AuraLogRegex();

    /// <summary>
    /// ログウォッチャー
    /// </summary>
    private readonly LogWatcher _watcher;

    /// <summary>
    /// 新しいAuraログを検出するサービス
    /// </summary>
    /// <param name="watcher">ログウォッチャー</param>
    public NewAuraDetectionService(LogWatcher watcher)
    {
        _watcher = watcher;
        _watcher.OnNewLogLine += HandleLogLine;
    }

    /// <summary>
    /// ログ行を処理する
    /// </summary>
    /// <param name="line">ログ行</param>
    /// <param name="isFirstReading">初回読み込みかどうか</param>
    private void HandleLogLine(string line, bool isFirstReading)
    {
        Match matchAuraLogPattern = AuraLogRegex().Match(line);
        Console.WriteLine($"NewAuraDetectionService.HandleLogLine/matchAuraLogPattern.Success: {matchAuraLogPattern.Success}");
        if (!matchAuraLogPattern.Success)
        {
            return;
        }

        var auraId = matchAuraLogPattern.Groups["AuraId"].Value;
        OnDetected.Invoke(Aura.GetAura(auraId), isFirstReading);
    }
}

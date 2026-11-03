using System.Globalization;
using System.Text.RegularExpressions;
using ElitesRNGAuraObserver.Core.VRChat;

namespace ElitesRNGAuraObserver.Core.Aura;

/// <summary>
/// 新しいAuraログを検出するサービス
/// </summary>
internal partial class NewAuraDetectionService
{
    /// <summary>
    /// Raised when an obtained Aura is detected. Arguments: the Aura, and whether the line was read while catching up from the start of the log file (first reading)
    /// </summary>
    public event Action<Aura, bool> OnDetected = (aura, isFirstReading) => { };

    /// <summary>
    /// Aura取得時のログパターン
    /// </summary>
    /// <example>2025.04.16 18:07:07 Debug      -  [&lt;color=#00EEFF&gt;Elite's RNG Land&lt;/color&gt;] [&lt;color=grey&gt;LOG&lt;/color&gt;] Successfully legitimized Aura #60.</example>
    [GeneratedRegex(@"(?<datetime>[0-9]{4}\.[0-9]{2}.[0-9]{2} [0-9]{2}:[0-9]{2}:[0-9]{2}) (?<Level>.[A-z]+) *- *\[<color=#00EEFF>Elite's RNG Land</color>] \[<color=grey>LOG</color>] Successfully legitimized Aura #(?<AuraId>[0-9]+)\.")]
    private static partial Regex AuraLogRegex();

    /// <summary>
    /// ログウォッチャー
    /// </summary>
    private readonly LogWatcher _watcher;

    /// <summary>
    /// 指定された LogWatcher を使用して新しいインスタンスを初期化する
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

        var auraId = int.Parse(matchAuraLogPattern.Groups["AuraId"].Value, CultureInfo.InvariantCulture);
        OnDetected.Invoke(Aura.GetAura(auraId), isFirstReading);
    }
}

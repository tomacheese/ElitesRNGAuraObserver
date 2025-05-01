using System.Diagnostics;
using System.Text.RegularExpressions;

namespace RNGNewAuraNotifier.Core.VRChat;
internal partial class AuthenticatedDetectionService
{
    public event Action<VRChatUser, bool> OnDetected = delegate { };
    /// <summary>
    /// VRChatログイン時のログパターン
    /// </summary>
    /// <example>2025.04.19 14:10:45 Debug      -  User Authenticated: Tomachi (usr_0b83d9be-9852-42dd-98e2-625062400acc)</example>
    [GeneratedRegex(@"(?<datetime>[0-9]{4}\.[0-9]{2}.[0-9]{2} [0-9]{2}:[0-9]{2}:[0-9]{2}) (?<Level>.[A-z]+) *- *User Authenticated: (?<UserName>.+) \((?<UserId>usr_[A-z0-9\-]+)\)")]
    private static partial Regex UserAuthenticatedRegex();

    /// <summary>
    /// ログウォッチャー
    /// </summary>
    private readonly LogWatcher _watcher;

    /// <summary>
    /// 新しいユーザーログインを検出するサービス
    /// </summary>
    /// <param name="watcher"></param>
    public AuthenticatedDetectionService(LogWatcher watcher)
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
        var matchUserLogPattern = UserAuthenticatedRegex().Match(line);
        Debug.WriteLine($"AuthenticatedDetectionService.HandleLogLine/matchUserLogPattern.Success: ${matchUserLogPattern.Success}");
        if (!matchUserLogPattern.Success)
        {
            return;
        }

        string userName = matchUserLogPattern.Groups["UserName"].Value;
        string userId = matchUserLogPattern.Groups["UserId"].Value;
        OnDetected.Invoke(new VRChatUser
        {
            UserName = userName,
            UserId = userId
        }, isFirstReading);
    }


}

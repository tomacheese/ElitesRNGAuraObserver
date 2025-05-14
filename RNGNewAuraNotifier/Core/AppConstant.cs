using System.Reflection;

namespace RNGNewAuraNotifier.Core;
internal class AppConstant
{
    /// <summary>
    /// アプリケーション名
    /// </summary>
    public static readonly string AppName = Assembly.GetExecutingAssembly().GetName().Name ?? string.Empty;

    /// <summary>
    /// アプリケーションバージョン
    /// </summary>
    public static readonly Version AppVersion = Assembly.GetExecutingAssembly().GetName().Version ?? new Version(0, 0, 0);

    /// <summary>
    /// VRChatのデフォルトログディレクトリのパス
    /// </summary>
    public static readonly string VRChatDefaultLogDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "AppData", "LocalLow", "VRChat", "VRChat");
}
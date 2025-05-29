using System.Reflection;

namespace RNGNewAuraNotifier.Core;

/// <summary>
/// アプリケーションの定数を格納するクラス
/// </summary>
internal static class AppConstants
{
    /// <summary>
    /// アプリケーション名
    /// </summary>
    public static readonly string AppName = Assembly.GetExecutingAssembly().GetName().Name ?? string.Empty;

    /// <summary>
    /// アプリケーションバージョンの文字列
    /// </summary>
    public static readonly string AppVersionString = (Assembly.GetExecutingAssembly().GetName().Version ?? new Version(0, 0, 0)).ToString(3); // Major.Minor.Patch

    /// <summary>
    /// VRChatのデフォルトログディレクトリのパス
    /// </summary>
    public static readonly string VRChatDefaultLogDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "AppData", "LocalLow", "VRChat", "VRChat");

    /// <summary>
    /// GitHub リポジトリのオーナー名
    /// </summary>
    public const string GitHubRepoOwner = "tomacheese";

    /// <summary>
    /// GitHub リポジトリ名
    /// </summary>
    public const string GitHubRepoName = "RNGNewAuraNotifier";
}

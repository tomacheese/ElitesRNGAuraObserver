namespace RNGNewAuraNotifier.Core;
internal class AppConstant
{
    /// <summary>
    /// VRChatのデフォルトログディレクトリのパス
    /// </summary>
    public static readonly string VRChatDefaultLogDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "AppData", "LocalLow", "VRChat", "VRChat");
}

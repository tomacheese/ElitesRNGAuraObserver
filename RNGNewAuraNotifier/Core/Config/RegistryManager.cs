using Microsoft.Win32;

namespace RNGNewAuraNotifier.Core.Config;

/// <summary>
/// Windowsスタートアップの管理クラス
/// </summary>
internal class RegistryManager
{
    /// <summary>
    /// アプリケーションをWindowsのスタートアップに登録するかどうかを設定します。
    /// </summary>
    /// <param name="enableStartup">設定の有効/無効</param>
    public static void SetStartup(bool enableStartup)
    {
        using RegistryKey? key = Registry.CurrentUser.OpenSubKey(AppConstants.StartupKeyPath, true);
        if (enableStartup)
        {
            var exePath = Application.ExecutablePath;
            key!.SetValue(AppConstants.AppName, "\"" + exePath + "\"");
        }
        else
        {
            key!.DeleteValue(AppConstants.AppName, false);
        }
    }

    /// <summary>
    /// アプリケーションがWindowsのスタートアップに登録されているかどうかを確認し、必要に応じて登録します。
    /// </summary>
    /// <param name="enableStartup">設定の有効/無効</param>
    public static void EnsureStartupRegistration(bool enableStartup)
    {
        using RegistryKey? key = Registry.CurrentUser.OpenSubKey(AppConstants.StartupKeyPath, true);
        var value = key!.GetValue(AppConstants.AppName);
        var currentExePath = $"\"{Application.ExecutablePath}\"";

        if (enableStartup || value == null || value.ToString() != currentExePath)
        {
            // 値が存在しない or パスが違う → 再登録
            key.SetValue(AppConstants.AppName, currentExePath);
        }
        else if (!enableStartup)
        {
            // スタートアップを無効にする場合は削除
            key.DeleteValue(AppConstants.AppName, false);
        }
    }
}

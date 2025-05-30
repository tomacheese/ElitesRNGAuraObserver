using Microsoft.Win32;

namespace RNGNewAuraNotifier.Core.Config;

/// <summary>
/// Windowsスタートアップの管理クラス
/// </summary>
internal class RegistryManager
{
    /// <summary>
    /// Windowsスタートアップに登録する際のレジストリキーのパス
    /// </summary>
    public const string StartupKeyPath = @"Software\Microsoft\Windows\CurrentVersion\Run";

    /// <summary>
    /// アプリケーションをWindowsのスタートアップに登録するかどうかを設定します。
    /// </summary>
    /// <param name="enableStartup">設定の有効/無効</param>
    public static void SetStartup(bool enableStartup)
    {
        using RegistryKey? key = Registry.CurrentUser.OpenSubKey(StartupKeyPath, true);
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
    public static void EnsureStartupRegistration()
    {
        using RegistryKey? key = Registry.CurrentUser.OpenSubKey(StartupKeyPath, true);
        var value = key!.GetValue(AppConstants.AppName);
        var currentExePath = $"\"{Application.ExecutablePath}\"";

        if (value == null || value.ToString() != currentExePath)
        {
            // 値が存在しない or パスが違う → 再登録
            key.SetValue(AppConstants.AppName, currentExePath);
        }
    }

    /// <summary>
    /// スタートアップにアプリケーションが登録されているかどうかを確認します。
    /// </summary>
    /// <returns>登録されていれば true、そうでなければ false</returns>
    public static bool IsRegisteredStartup()
    {
        using RegistryKey? key = Registry.CurrentUser.OpenSubKey(StartupKeyPath, false);
        if (key == null) return false;
        var value = key.GetValue(AppConstants.AppName);
        return value != null;
    }
}

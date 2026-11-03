using Microsoft.Win32;

namespace ElitesRNGAuraObserver.Core.Config;

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
            key!.SetValue(AppConstants.AssemblyName, "\"" + exePath + "\"");
        }
        else
        {
            key!.DeleteValue(AppConstants.AssemblyName, false);
        }
    }

    /// <summary>
    /// スタートアップのレジストリ値が別のパスで登録済みの場合、現在の実行ファイルのパスに更新します。値が存在しない場合は何もしません。
    /// </summary>
    public static void EnsureStartupRegistration()
    {
        using RegistryKey? key = Registry.CurrentUser.OpenSubKey(StartupKeyPath, true);
        var value = key!.GetValue(AppConstants.AssemblyName);
        var currentExePath = $"\"{Application.ExecutablePath}\"";

        if (value == null) return;

        if (value.ToString() != currentExePath)
        {
            // パスが違う → 現在のパスで再登録
            key.SetValue(AppConstants.AssemblyName, currentExePath);
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
        var value = key.GetValue(AppConstants.AssemblyName);
        return value != null;
    }
}

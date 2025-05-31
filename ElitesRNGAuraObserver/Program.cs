using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;
using ElitesRNGAuraObserver.Core;
using ElitesRNGAuraObserver.Core.Config;
using ElitesRNGAuraObserver.Core.Json;
using ElitesRNGAuraObserver.Core.Updater;
using ElitesRNGAuraObserver.UI.TrayIcon;
using Microsoft.Toolkit.Uwp.Notifications;

namespace ElitesRNGAuraObserver;

/// <summary>
/// ElitesRNGAuraObserverのエントリポイント
/// </summary>
internal static partial class Program
{
    /// <summary>
    /// AuraObserverControllerのインスタンス
    /// </summary>
    private static AuraObserverController? _controller;

    /// <summary>
    /// Win32 APIのAllocConsole関数をインポートして、デバッグコンソールを割り当てる
    /// </summary>
    /// <returns>割り当てに成功した場合はtrue、それ以外はfalse</returns>
    [LibraryImport("kernel32.dll", SetLastError = true)]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool AllocConsole();

    /// <summary>
    /// アプリケーションのエントリポイント
    /// </summary>
    [STAThread]
    public static void Main()
    {
        if (ToastNotificationManagerCompat.WasCurrentProcessToastActivated())
        {
            // トースト通知から起動された場合、なにもしない
            ToastNotificationManagerCompat.Uninstall();
            return;
        }

        // 例外処理ハンドラを登録
        RegisterExceptionHandlers();

        var cmds = Environment.GetCommandLineArgs();
        // デバッグコンソールの設定
        HandleDebugConsole(cmds);
        // アップデートチェック
        UpdateCheck(cmds);

        Console.WriteLine("Program.Main");

        RegistryManager.EnsureStartupRegistration();
        CheckExistsLogDirectory();

        ApplicationConfiguration.Initialize();
        ConfigData configData = AppConfig.Instance;
        _controller = new AuraObserverController(configData);
        _controller.Start();

        Application.ApplicationExit += (s, e) =>
        {
            Console.WriteLine("Program.ApplicationExit");
            _controller?.Dispose();
            ToastNotificationManagerCompat.Uninstall();
        };

        Application.Run(new TrayIcon());
    }

    /// <summary>
    /// 例外ハンドラを登録するメソッド
    /// </summary>
    private static void RegisterExceptionHandlers()
    {
        Application.ThreadException += (s, e) => OnException(e.Exception, "ThreadException");
        Thread.GetDomain().UnhandledException += (s, e) => OnException((Exception)e.ExceptionObject, "UnhandledException");
        TaskScheduler.UnobservedTaskException += (s, e) => OnException(e.Exception, "UnobservedTaskException");
    }

    /// <summary>
    /// デバッグコンソールを有効にするメソッド
    /// </summary>
    /// <param name="cmds">アプリケーションの起動引数</param>
    private static void HandleDebugConsole(string[] cmds)
    {
        if (cmds.Any(cmd => cmd.Equals("--debug")))
        {
            AllocConsole();
            Console.SetOut(new StreamWriter(Console.OpenStandardOutput()) { AutoFlush = true });
            Console.OutputEncoding = Encoding.UTF8;
        }
    }

    /// <summary>
    /// ログディレクトリの存在を確認し、存在しない場合はデフォルト値にリセットするメソッド
    /// </summary>
    private static void CheckExistsLogDirectory()
    {
        ConfigData configData = AppConfig.Instance;
        // ログディレクトリのパス対象が存在しない場合はメッセージを出してリセットする
        if (!Directory.Exists(configData.LogDir))
        {
            MessageBox.Show(
                string.Join("\n", new List<string>()
                {
                    "The log directory does not exist.",
                    "Log directory settings return to default value.",
                }),
                "Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);

            configData.LogDir = AppConstants.VRChatLogDirectory;
            AppConfig.Save();
        }
    }

    /// <summary>
    /// アップデートチェックを行うメソッド
    /// </summary>
    /// <param name="cmds">アプリケーションの起動引数</param>
    private static void UpdateCheck(string[] cmds)
    {
        if (cmds.Any(cmd => cmd.Equals("--skip-update")))
        {
            Console.WriteLine("Skip update check");
        }
        else
        {
            Task.Run(async () =>
            {
                await JsonData.GetLatestJsonDataAsync().ConfigureAwait(false);
                var existsUpdate = await UpdateChecker.CheckAsync().ConfigureAwait(false);
                if (existsUpdate)
                {
                    Console.WriteLine("Found update. Exiting...");
                    return;
                }
            }).Wait();
        }
    }

    /// <summary>
    /// 例外が発生したときに呼び出されるメソッド
    /// </summary>
    /// <param name="e">Exception</param>
    /// <param name="exceptionType">例外の発生元種類</param>
    public static void OnException(Exception e, string exceptionType)
    {
        Console.WriteLine($"Exception: {exceptionType}");
        Console.WriteLine($"Message: {e.Message}");
        Console.WriteLine($"InnerException: {e.InnerException?.Message}");
        Console.WriteLine($"StackTrace: {e.StackTrace}");
        Console.WriteLine($"InnerException StackTrace: {e.InnerException?.StackTrace}");

        DialogResult result = MessageBox.Show(
            string.Join("\n", new List<string>()
            {
                "An error has occurred and the operation has stopped.",
                "It would be helpful if you could report this bug using GitHub issues!",
                $"https://github.com/{AppConstants.GitHubRepoOwner}/{AppConstants.GitHubRepoName}/issues",
                string.Empty,
                GetErrorDetails(e, false),
                string.Empty,
                "Click OK to open the Create GitHub issue page.",
                "Click Cancel to close this application.",
            }),
            $"Error ({exceptionType})",
            MessageBoxButtons.OKCancel,
            MessageBoxIcon.Error);

        if (result == DialogResult.OK)
        {
            Process.Start(new ProcessStartInfo()
            {
                FileName = $"https://github.com/{AppConstants.GitHubRepoOwner}/{AppConstants.GitHubRepoName}/issues/new?body={Uri.EscapeDataString(GetErrorDetails(e, true))}",

                UseShellExecute = true,
            });
        }

        Application.Exit();
    }

    /// <summary>
    /// 例外の詳細情報を取得するメソッド
    /// </summary>
    /// <param name="e">Exception</param>
    /// <param name="isMarkdown">Markdown形式で出力するかどうか</param>
    /// <returns>例外の詳細情報</returns>
    private static string GetErrorDetails(Exception e, bool isMarkdown)
    {
        var sb = new StringBuilder();
        var fence = isMarkdown ? "```" : string.Empty;

        void AppendSection(string title, string content)
        {
            if (isMarkdown)
            {
                sb.AppendLine(CultureInfo.InvariantCulture, $"## {title}\n\n{fence}\n{content}\n{fence}\n");
            }
            else
            {
                sb.AppendLine(CultureInfo.InvariantCulture, $"----- {title} -----\n{content}\n");
            }
        }

        Exception? current = e;
        var level = 0;
        while (current != null)
        {
            var title = level == 0
                ? "Error"
                : $"Inner Exception (Level {level})";
            AppendSection(title, $"{current.Message ?? "<no message>"}\n{current.StackTrace ?? "<no trace>"}");

            current = current.InnerException;
            level++;
        }

        // Environment info
        var content = $"""
            OS: {Environment.OSVersion}
            CLR: {Environment.Version}
            App: {AppConstants.AppName} {AppConstants.AppVersionString}
            """;
        AppendSection("Environment", content);

        return sb.ToString().Trim();
    }

    /// <summary>
    /// AuraObserverControllerのインスタンスを取得する
    /// </summary>
    /// <returns>AuraObserverControllerのインスタンス</returns>
    public static AuraObserverController? GetController() => _controller;

    /// <summary>
    /// AuraObserverControllerを再起動する
    /// </summary>
    /// <param name="configData">アプリケーション設定</param>
    public static void RestartController(ConfigData configData)
    {
        _controller?.Dispose();
        _controller = new AuraObserverController(configData);
        _controller.Start();
    }
}

using RNGNewAuraNotifier.Core;
using RNGNewAuraNotifier.Core.Config;
using RNGNewAuraNotifier.UI.TrayIcon;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace RNGNewAuraNotifier;
internal static partial class Program
{
    public static RNGNewAuraController? Controller;

    [LibraryImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool AllocConsole();

    [STAThread]
    static void Main()
    {
        Console.WriteLine("RNGNewAuraNotifier.Main");

        Application.ThreadException += new
            ThreadExceptionEventHandler(Application_ThreadException);

        // UnhandledExceptionイベント・ハンドラを登録する
        Thread.GetDomain().UnhandledException += new
            UnhandledExceptionEventHandler(Application_UnhandledException);

        string[] cmds = Environment.GetCommandLineArgs();
        bool isDebugMode = false;
        foreach (string cmd in cmds)
        {
            if (cmd.Equals("--debug"))
            {
                isDebugMode = true;
            }
        }
        if (isDebugMode)
        {
            AllocConsole();
            Console.SetOut(new StreamWriter(Console.OpenStandardOutput()) { AutoFlush = true });
        }

        Console.WriteLine("hoge");

        // ログディレクトリのパス対象が存在しない場合はメッセージを出してリセットする
        if (!Directory.Exists(AppConfig.LogDir))
        {
            MessageBox.Show(
                "The log directory does not exist.\n" +
                "Log directory settings return to default value.",
                "Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);

            AppConfig.LogDir = Path.GetFullPath(Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                @"..\LocalLow\VRChat\VRChat"
            ));
        }

        Controller = new RNGNewAuraController(AppConfig.LogDir);
        Controller.Start();

        ApplicationConfiguration.Initialize();
        Application.Run(new TrayIcon());
    }

    public static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
    {
        DialogResult result = MessageBox.Show(
            "An error has occurred and the operation has stopped.\n" +
            "It would be helpful if you could report this bug using GitHub issues!\n" +
            "https://github.com/tomacheese/VRCXDiscordTracker/issues\n" +
            "\n" +
            "----- Error Details -----\n" +
            e.Exception.Message + "\n" +
            e.Exception.InnerException?.Message + "\n" +
            "\n" +
            "----- StackTrace -----\n" +
            e.Exception.StackTrace + "\n" +
            "\n" +
            "Click OK to open the Create GitHub issue page.\n" +
            "Click Cancel to close this application.",
            "Error (ThreadException)",
            MessageBoxButtons.OKCancel,
            MessageBoxIcon.Error);

        if (result == DialogResult.OK)
        {
            Process.Start("https://github.com/tomacheese/VRCXDiscordTracker/issues/new");
        }
        Application.Exit();
    }

    public static void Application_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        Exception? ex = e.ExceptionObject as Exception;
        if (ex == null)
        {
            return;
        }
        DialogResult result = MessageBox.Show(
            "An error has occurred and the operation has stopped.\n" +
            "It would be helpful if you could report this bug using GitHub issues!\n" +
            "https://github.com/tomacheese/VRCXDiscordTracker/issues\n" +
            "\n" +
            "----- Error Details -----\n" +
            ex.Message + "\n" +
            ex.InnerException?.Message + "\n" +
            "\n" +
            "----- StackTrace -----\n" +
            ex.StackTrace + "\n" +
            "\n" +
            "Click OK to open the Create GitHub issue page.\n" +
            "Click Cancel to close this application.",
            "Error (UnhandledException)",
            MessageBoxButtons.OKCancel,
            MessageBoxIcon.Error);

        if (result == DialogResult.OK)
        {
            Process.Start("https://github.com/tomacheese/VRCXDiscordTracker/issues/new");
        }
        Application.Exit();
    }
}
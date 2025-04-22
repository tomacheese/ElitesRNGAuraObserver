using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace RNGNewAuraNotifier
{
    internal static class Program
    {
        [DllImport("shell32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern int SetCurrentProcessExplicitAppUserModelID(string AppID);

        public static VRChatLogWatcher Watcher { get; set; }

        [STAThread]
        static void Main()
        {
            SetCurrentProcessExplicitAppUserModelID("Tomacheese.RNGNewAuraNotifier");

            // VRChatのログを監視
            string targetLogDirPath = AppConfig.LogDir;
            Watcher = new VRChatLogWatcher(targetLogDirPath);
            Watcher.Start();

            // システムトレイアイコンを作成
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new RNGNewAuraNotifier());
        }
    }
}

using RNGNewAuraNotifier.Core;
using RNGNewAuraNotifier.Core.Config;
using RNGNewAuraNotifier.UI.TrayIcon;
using System.Diagnostics;

namespace RNGNewAuraNotifier;
internal static class Program
{
    public static RNGNewAuraController Controller = new(AppConfig.LogDir);

    [STAThread]
    static void Main()
    {
        Debug.WriteLine("RNGNewAuraNotifier.Main");
        Controller.Start();

        ApplicationConfiguration.Initialize();
        Application.Run(new TrayIcon());
    }
}
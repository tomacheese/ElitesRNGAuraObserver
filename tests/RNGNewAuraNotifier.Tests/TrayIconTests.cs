using System.Reflection;
using System.Windows.Forms;

namespace RNGNewAuraNotifier.Tests
{
    public class TrayIconTests
    {
        // ShowSettingsが呼び出された際に設定フォームが作成され表示されることを確認します。
        [Fact]
        public void ShowSettings_CreatesAndShowsForm()
        {
            var context = new RNGNewAuraNotifier();
            // ensure initial settingsForm is null
            var settingsFormField = typeof(RNGNewAuraNotifier).GetField("settingsForm", BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.Null(settingsFormField.GetValue(context));

            // invoke ShowSettings
            var showSettings = typeof(RNGNewAuraNotifier).GetMethod("ShowSettings", BindingFlags.NonPublic | BindingFlags.Instance);
            showSettings.Invoke(context, new object[] { null, EventArgs.Empty });

            var form = settingsFormField.GetValue(context) as Form;
            Assert.NotNull(form);
            Assert.True(form.Visible);

            // cleanup
            form.Dispose();
        }

        // ShowSettingsが呼び出された際に既存のフォームが再利用されることを確認します。
        [Fact]
        public void ShowSettings_ReusesExistingForm()
        {
            var context = new RNGNewAuraNotifier();
            var settingsFormField = typeof(RNGNewAuraNotifier).GetField("settingsForm", BindingFlags.NonPublic | BindingFlags.Instance);
            var showSettings = typeof(RNGNewAuraNotifier).GetMethod("ShowSettings", BindingFlags.NonPublic | BindingFlags.Instance);

            showSettings.Invoke(context, new object[] { null, EventArgs.Empty });
            var first = settingsFormField.GetValue(context);
            showSettings.Invoke(context, new object[] { null, EventArgs.Empty });
            var second = settingsFormField.GetValue(context);

            Assert.Same(first, second);
            (first as Form)?.Dispose();
        }

        // Exitが呼び出された際にトレイアイコンが破棄され非表示になることを確認します。
        [Fact]
        public void Exit_DisposesTrayIconAndHides()
        {
            var context = new RNGNewAuraNotifier();
            var trayField = typeof(RNGNewAuraNotifier).GetField("trayIcon", BindingFlags.NonPublic | BindingFlags.Instance);
            var trayIcon = trayField.GetValue(context) as NotifyIcon;
            Assert.True(trayIcon.Visible);

            var exitMethod = typeof(RNGNewAuraNotifier).GetMethod("Exit", BindingFlags.NonPublic | BindingFlags.Instance);
            exitMethod.Invoke(context, new object[] { null, EventArgs.Empty });

            Assert.False(trayIcon.Visible);
            // Disposing again should not throw
            trayIcon.Dispose();
        }
    }
}
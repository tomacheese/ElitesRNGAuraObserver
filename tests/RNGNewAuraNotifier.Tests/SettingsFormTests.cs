using System.Reflection;
using System.Windows.Forms;

namespace RNGNewAuraNotifier.Tests
{
    public class SettingsFormTests : IDisposable
    {
        private readonly string _tempDir;

        public SettingsFormTests()
        {
            _tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(_tempDir);
        }

        public void Dispose()
        {
            try { Directory.Delete(_tempDir, true); } catch { }
        }

        [Fact]
        // AppConfigの設定が正しくフォームに読み込まれることを確認します。
        public void OnLoad_UsesAppConfigWhenSet()
        {
            var cfgDir = _tempDir;
            AppConfig.LogDir = cfgDir;
            AppConfig.DiscordWebhookUrl = "http://example.com";
            Program.Watcher = new VRChatLogWatcher(null);

            var form = new SettingsForm();
            // invoke OnLoad
            var onLoad = typeof(SettingsForm).GetMethod("OnLoad", BindingFlags.NonPublic | BindingFlags.Instance);
            onLoad.Invoke(form, new object[] { null, EventArgs.Empty });

            // access text boxes
            var tbLogDir = (TextBox)typeof(SettingsForm).GetField("textBoxLogDir", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(form);
            var tbUrl = (TextBox)typeof(SettingsForm).GetField("textBoxDiscordWebhookUrl", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(form);

            Assert.Equal(cfgDir, tbLogDir.Text);
            Assert.Equal("http://example.com", tbUrl.Text);
        }

        [Fact]
        // AppConfigが設定されていない場合、Watcherの設定がフォームに読み込まれることを確認します。
        public void OnLoad_UsesWatcherWhenNoAppConfig()
        {
            AppConfig.LogDir = null;
            Program.Watcher = new VRChatLogWatcher(_tempDir);
            // set current file
            var getCurrent = typeof(VRChatLogWatcher).GetMethod("GetCurrentFile", BindingFlags.Public | BindingFlags.Instance);
            // no file, so null

            var form = new SettingsForm();
            var onLoad = typeof(SettingsForm).GetMethod("OnLoad", BindingFlags.NonPublic | BindingFlags.Instance);
            onLoad.Invoke(form, new object[] { null, EventArgs.Empty });

            var tbLogDir = (TextBox)typeof(SettingsForm).GetField("textBoxLogDir", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(form);
            Assert.Equal(_tempDir, tbLogDir.Text);
        }

        [Fact]
        // 有効な入力を保存するとWatcherが再起動し、AppConfigが更新されることを確認します。
        public void Save_ValidInput_RestartsWatcherAndReturnsTrue()
        {
            AppConfig.LogDir = null;
            AppConfig.DiscordWebhookUrl = null;
            Program.Watcher = new VRChatLogWatcher(_tempDir);

            var form = new SettingsForm();
            var onLoad = typeof(SettingsForm).GetMethod("OnLoad", BindingFlags.NonPublic | BindingFlags.Instance);
            onLoad.Invoke(form, new object[] { null, EventArgs.Empty });

            // set new values
            var tbLogDir = (TextBox)typeof(SettingsForm).GetField("textBoxLogDir", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(form);
            var tbUrl = (TextBox)typeof(SettingsForm).GetField("textBoxDiscordWebhookUrl", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(form);
            var newDir = _tempDir;
            tbLogDir.Text = newDir;
            tbUrl.Text = "https://webhook";

            var save = typeof(SettingsForm).GetMethod("Save", BindingFlags.NonPublic | BindingFlags.Instance);
            bool result = (bool)save.Invoke(form, null);

            Assert.True(result);
            Assert.Equal(newDir, AppConfig.LogDir);
            Assert.Equal("https://webhook", AppConfig.DiscordWebhookUrl);
            Assert.Equal(newDir, Program.Watcher.GetLogDir());
        }

        [Fact]
        // 無効なログディレクトリを保存するとfalseが返されることを確認します。
        public void Save_InvalidLogDir_ReturnsFalse()
        {
            var form = new SettingsForm();
            var onLoad = typeof(SettingsForm).GetMethod("OnLoad", BindingFlags.NonPublic | BindingFlags.Instance);
            onLoad.Invoke(form, new object[] { null, EventArgs.Empty });

            var tbLogDir = (TextBox)typeof(SettingsForm).GetField("textBoxLogDir", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(form);
            tbLogDir.Text = "Z:\\nonexistent";

            var save = typeof(SettingsForm).GetMethod("Save", BindingFlags.NonPublic | BindingFlags.Instance);
            bool result = (bool)save.Invoke(form, null);

            Assert.False(result);
        }

        [Fact]
        // 無効なDiscord webhook URLを保存するとfalseが返されることを確認します。
        public void Save_InvalidDiscordWebhookUrl_ReturnsFalse()
        {
            var form = new SettingsForm();
            var onLoad = typeof(SettingsForm).GetMethod("OnLoad", BindingFlags.NonPublic | BindingFlags.Instance);
            onLoad.Invoke(form, new object[] { null, EventArgs.Empty });

            var tbUrl = (TextBox)typeof(SettingsForm).GetField("textBoxDiscordWebhookUrl", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(form);
            tbUrl.Text = "invalidurl";

            var save = typeof(SettingsForm).GetMethod("Save", BindingFlags.NonPublic | BindingFlags.Instance);
            bool result = (bool)save.Invoke(form, null);

            Assert.False(result);
        }
    }
}
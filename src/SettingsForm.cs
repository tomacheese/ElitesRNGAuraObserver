using System.Windows.Forms;

namespace RNGNewAuraNotifier
{
    public partial class SettingsForm : Form
    {
        public SettingsForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 設定画面がロードされたときの処理
        /// <summary>
        private void OnLoad(object sender, System.EventArgs e)
        {
            // 設定ファイルから値を読み込む
            textBoxLogDir.Text = AppConfig.LogDir ?? Program.Watcher.GetLogDir(); // 設定ファイルで規定していない場合は実際の監視対象を取得
            textBoxDiscordWebhookUrl.Text = AppConfig.DiscordWebhookUrl;

            // 1秒ごとに監視対象パスの更新を行う
            var timer = new Timer();
            timer.Interval = 1000; // 1 sec
            timer.Tick += (s, args) =>
            {
                textBoxWatchingFilePath.Text = Program.Watcher.GetCurrentFile();
            };
            timer.Start();
        }

        /// <summary>
        /// 保存ボタンがクリックされたときの処理
        /// </summary>
        /// <remarks>設定ファイルに値を保存し、VRChatのログ監視を再起動する</remarks>
        private void OnSaveButtonClicked(object sender, System.EventArgs e)
        {
            AppConfig.LogDir = textBoxLogDir.Text != "" ? textBoxLogDir.Text : null;
            AppConfig.DiscordWebhookUrl = textBoxDiscordWebhookUrl.Text != "" ? textBoxDiscordWebhookUrl.Text : null;

            Program.Watcher.Stop();
            Program.Watcher = new VRChatLogWatcher(textBoxLogDir.Text);
        }
    }
}

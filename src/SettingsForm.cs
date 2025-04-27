using System;
using System.Windows.Forms;

namespace RNGNewAuraNotifier
{
    public partial class SettingsForm : Form
    {
        private Timer timer;
        private string lastSavedLogDir;
        private string lastSavedDiscordWebhookUrl;

        public SettingsForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 設定画面がロードされたときの処理
        /// </summary>
        private void OnLoad(object sender, EventArgs e)
        {
            // 設定ファイルから値を読み込む
            textBoxLogDir.Text = AppConfig.LogDir ?? Program.Watcher.GetLogDir(); // 設定ファイルで規定していない場合は実際の監視対象を取得
            textBoxDiscordWebhookUrl.Text = AppConfig.DiscordWebhookUrl;

            // 1秒ごとに監視対象パスの更新を行う
            timer = new Timer
            {
                Interval = 1000 // 1 sec
            };
            timer.Tick += (s, args) =>
            {
                textBoxWatchingFilePath.Text = Program.Watcher.GetCurrentFile();
            };
            timer.Start();

            lastSavedLogDir = textBoxLogDir.Text;
            lastSavedDiscordWebhookUrl = textBoxDiscordWebhookUrl.Text;
        }

        private bool Save()
        {
            try
            {
                AppConfig.LogDir = !string.IsNullOrEmpty(textBoxLogDir.Text) ? textBoxLogDir.Text : null;
                AppConfig.DiscordWebhookUrl = !string.IsNullOrEmpty(textBoxDiscordWebhookUrl.Text) ? textBoxDiscordWebhookUrl.Text : null;

                Program.Watcher.Stop();
                Program.Watcher = new VRChatLogWatcher(textBoxLogDir.Text);
                Program.Watcher.Start();

                lastSavedLogDir = textBoxLogDir.Text;
                lastSavedDiscordWebhookUrl = textBoxDiscordWebhookUrl.Text;

                Notifier.ShowToast("Settings Saved", "Settings have been saved successfully.");
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving settings: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        /// <summary>
        /// 保存ボタンがクリックされたときの処理
        /// </summary>
        /// <remarks>設定ファイルに値を保存し、VRChatのログ監視を再起動する</remarks>
        private void OnSaveButtonClicked(object sender, EventArgs e)
        {
            Save();
        }

        private void OnFormClosing(object sender, FormClosingEventArgs e)
        {
            var changed = lastSavedLogDir != textBoxLogDir.Text || lastSavedDiscordWebhookUrl != textBoxDiscordWebhookUrl.Text;
            if (!changed)
            {
                return;
            }

            var result = MessageBox.Show("Some settings are not saved. Do you want to save them?", "Confirm", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                var saved = Save();
                if (!saved)
                {
                    e.Cancel = true; // 保存に失敗した場合は閉じない
                }
            }
            else if (result == DialogResult.Cancel)
            {
                e.Cancel = true;
            }
        }

        private void OnFormClosed(object sender, FormClosedEventArgs e)
        {
            timer.Dispose();
        }
    }
}

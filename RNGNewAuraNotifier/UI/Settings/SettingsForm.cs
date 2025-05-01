using RNGNewAuraNotifier.Core;
using RNGNewAuraNotifier.Core.Config;
using RNGNewAuraNotifier.Core.Notification;
using Timer = System.Windows.Forms.Timer;

namespace RNGNewAuraNotifier.UI.Settings;
public partial class SettingsForm : Form
{
    private readonly Timer _timer = new()
    {
        Interval = 1000 // 1 sec
    };
    private string _lastSavedLogDir = string.Empty;
    private string _lastSavedDiscordWebhookUrl = string.Empty;

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
        textBoxLogDir.Text = AppConfig.LogDir;
        if (string.IsNullOrWhiteSpace(textBoxLogDir.Text))
        {
            textBoxLogDir.Text = Program.Controller.GetLogDirectory();
        }
        textBoxDiscordWebhookUrl.Text = AppConfig.DiscordWebhookUrl;

        // 1秒ごとに監視対象パスの更新を行う
        _timer.Tick += (s, args) =>
        {
            textBoxWatchingFilePath.Text = Program.Controller.GetLastReadFilePath();
        };
        _timer.Start();

        _lastSavedLogDir = textBoxLogDir.Text;
        _lastSavedDiscordWebhookUrl = textBoxDiscordWebhookUrl.Text;
    }

    /// <summary>
    /// 設定を保存するメソッド
    /// </summary>
    /// <returns>設定が保存出来たかどうか</returns>
    private bool Save()
    {
        try
        {
            AppConfig.LogDir = textBoxLogDir.Text;
            AppConfig.DiscordWebhookUrl = textBoxDiscordWebhookUrl.Text;

            Program.Controller.Dispose();
            Program.Controller = new RNGNewAuraController(textBoxLogDir.Text);
            Program.Controller.Start();

            _lastSavedLogDir = textBoxLogDir.Text;
            _lastSavedDiscordWebhookUrl = textBoxDiscordWebhookUrl.Text;

            UwpNotificationService.Notify("Settings Saved", "Settings have been saved successfully.");
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

    /// <summary>
    /// フォームが閉じられるときの処理
    /// </summary>
    private void OnFormClosing(object sender, FormClosingEventArgs e)
    {
        var changed = _lastSavedLogDir != textBoxLogDir.Text.Trim() || _lastSavedDiscordWebhookUrl != textBoxDiscordWebhookUrl.Text.Trim();
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

    /// <summary>
    /// フォームが閉じられたときの処理
    /// </summary>
    private void OnFormClosed(object sender, FormClosedEventArgs e)
    {
        _timer.Dispose();
    }
}

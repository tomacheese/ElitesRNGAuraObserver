using RNGNewAuraNotifier.Core;
using RNGNewAuraNotifier.Core.Config;
using RNGNewAuraNotifier.Core.Json;
using RNGNewAuraNotifier.Core.Notification;
using Timer = System.Windows.Forms.Timer;

namespace RNGNewAuraNotifier.UI.Settings;

/// <summary>
/// 設定画面のフォームクラス
/// </summary>
internal partial class SettingsForm : Form
{
    /// <summary>
    /// 監視対象パス更新タイマー
    /// </summary>
    private readonly Timer _timer = new()
    {
        Interval = 1000, // 1 sec
    };

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public SettingsForm() => InitializeComponent();

    /// <summary>
    /// 設定画面がロードされたときの処理
    /// </summary>
    private void OnLoad(object sender, EventArgs e)
    {
        ActiveControl = null;
        ConfigData configData = AppConfig.Instance;

        // フォーム内のテキストボックスは、フォーカスされたときに全選択する
        foreach (TextBox tb in Controls.OfType<TextBox>())
        {
            tb.Enter += TextBox_Enter;
        }

        // JSONのバージョン情報を取得
        labelJsonVersion.Text = JsonData.GetVersion();
        labelAppVersion.Text = AppConstants.AppVersionString;

        // 設定値を反映
        textBoxDiscordWebhookUrl.Text = configData.DiscordWebhookUrl;
        checkBoxToastNotification.Checked = configData.ToastNotification;
        textBoxConfigDir.Text = AppConfig.GetConfigDirectoryPath();

        // Windowsのスタートアップに登録されているかどうかをチェック
        checkBoxStartup.Checked = RegistryManager.IsStartupRegistered();

        // 1秒ごとに監視対象パスの更新を行う
        _timer.Tick += (s, args) =>
        {
            textBoxWatchingFilePath.Text = Program.GetController()?.GetLastReadFilePath() ?? string.Empty;
        };
        _timer.Start();
    }

    /// <summary>
    /// 設定を保存するメソッド
    /// </summary>
    /// <returns>設定が保存出来たかどうか</returns>
    private bool Save()
    {
        try
        {
            AppConfig.SaveConfigDirectoryPath(textBoxConfigDir.Text.Trim());

            ConfigData configData = AppConfig.Instance;
            configData.DiscordWebhookUrl = textBoxDiscordWebhookUrl.Text;
            configData.ToastNotification = checkBoxToastNotification.Checked;
            AppConfig.Save();
            Program.RestartController(configData);

            UwpNotificationService.Notify("Settings Saved", "Settings have been saved successfully.");
            return true;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error saving settings: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return false;
        }
        finally
        {
            RegistryManager.SetStartup(checkBoxStartup.Checked);
        }
    }

    /// <summary>
    /// 保存ボタンがクリックされたときの処理
    /// </summary>
    /// <remarks>設定ファイルに値を保存し、VRChatのログ監視を再起動する</remarks>
    private void OnSaveButtonClicked(object sender, EventArgs e) => Save();

    /// <summary>
    /// フォームが閉じられるときの処理
    /// </summary>
    private void OnFormClosing(object sender, FormClosingEventArgs e)
    {
        ConfigData configData = AppConfig.Instance;
        var changedConfigData = new ConfigData
        {
            DiscordWebhookUrl = textBoxDiscordWebhookUrl.Text.Trim(),
            ToastNotification = checkBoxToastNotification.Checked,
        };

        if (ConfigData.AreEqual(configData, changedConfigData))
        {
            return;
        }

        DialogResult result = MessageBox.Show("Some settings are not saved. Do you want to save them?", "Confirm", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
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
    private void OnFormClosed(object sender, FormClosedEventArgs e) => _timer.Dispose();

    /// <summary>
    /// テキストボックスにフォーカスが当たった時、中身の文字列を全選択する
    /// </summary>
    private void TextBox_Enter(object? sender, EventArgs e)
    {
        if (sender is TextBox textBox)
        {
            BeginInvoke(new Action(textBox.SelectAll));
        }
    }

    /// <summary>
    /// コンフィグディレクトリの参照ボタンがクリックされたときの処理
    /// </summary>
    private void ButtonConfigDirBrowse_Click(object sender, EventArgs e)
    {
        folderBrowserDialog.SelectedPath = textBoxConfigDir.Text.Trim();

        if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
        {
            textBoxConfigDir.Text = folderBrowserDialog.SelectedPath;
        }
    }

    /// <summary>
    /// テスト送信ボタンがクリックされたときの処理
    /// </summary>
    private void ButtonSendTest_Click(object sender, EventArgs e)
    {
        Task.Run(async () =>
        {
            await DiscordNotificationService.NotifyAsync(
                discordWebhookUrl: textBoxDiscordWebhookUrl.Text,
                title: "**Test Message**",
                message: "This is a test message.",
                vrchatUser: null
            ).ConfigureAwait(false);
        }).Wait();
    }
}

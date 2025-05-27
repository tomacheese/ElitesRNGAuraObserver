using System.Text.Json.Serialization;

namespace RNGNewAuraNotifier.Core.Config;

/// <summary>
/// 設定データを格納するクラス
/// </summary>
/// <remarks>JSON形式でシリアライズされる</remarks>
internal class ConfigData
{
    /// <summary>
    /// VRChatのログディレクトリのパス
    /// </summary>
    /// <remarks>デフォルトは %USERPROFILE%\AppData\LocalLow\VRChat\VRChat</remarks>
    [JsonPropertyName("logDir")]
    public string LogDir { get; set; } = Environment.ExpandEnvironmentVariables(@"%USERPROFILE%\AppData\LocalLow\VRChat\VRChat");

    /// <summary>
    /// DiscordのWebhook URL
    /// </summary>
    [JsonPropertyName("discordWebhookUrl")]
    public string DiscordWebhookUrl { get; set; } = string.Empty;

    /// <summary>
    /// トースト通知の有効/無効
    /// </summary>
    /// <remarks>デフォルトは true(有効)</remarks>
    [JsonPropertyName("toastNotification")]
    public bool ToastNotification { get; set; } = true;

    /// <summary>
    /// Windowsスタートアップの有効/無効
    /// </summary>
    /// <remarks>デフォルトは false(無効)</remarks>
    [JsonPropertyName("windowsStartup")]
    public bool WindowsStartup { get; set; } = false;

    /// <summary>
    /// 設定ファイルのディレクトリのパス
    /// </summary>
    [JsonPropertyName("configDir")]
    public string ConfigDir { get; set; } = Environment.ExpandEnvironmentVariables(@$"%USERPROFILE%\AppData\Local\{AppConstants.AppName}");
}

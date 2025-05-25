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
    public string LogDir { get; set; } = string.Empty;

    /// <summary>
    /// DiscordのWebhook URL
    /// </summary>
    [JsonPropertyName("discordWebhookUrl")]
    public string DiscordWebhookUrl { get; set; } = string.Empty;
}

using System.Reflection;
using System.Text.Json.Serialization;

namespace RNGNewAuraNotifier.Core.Config;

/// <summary>
/// 設定データを格納するクラス
/// </summary>
/// <remarks>JSON形式でシリアライズされる</remarks>
internal class ConfigData
{
    private string _discordWebhookUrl = string.Empty;

    /// <summary>
    /// DiscordのWebhook URL
    /// </summary>
    [JsonPropertyName("discordWebhookUrl")]
    public string DiscordWebhookUrl
    {
        get => _discordWebhookUrl;
        set
        {
            if (!string.IsNullOrEmpty(value) && !IsValidUrl(value))
            {
                throw new ArgumentException("AppUrl must start with 'http://' or 'https://'.");
            }

            _discordWebhookUrl = value;
        }
    }

    /// <summary>
    /// トースト通知の有効/無効
    /// </summary>
    /// <remarks>デフォルトは true(有効)</remarks>
    [JsonPropertyName("toastNotification")]
    public bool ToastNotification { get; set; } = true;

    /// <summary>
    /// VRChatのログディレクトリ
    /// </summary>
    [JsonPropertyName("logDir")]
    public string LogDir { get; set; } = AppConstants.VRChatLogDirectory;

    /// <summary>
    /// Aura.jsonのパス
    /// </summary>
    [JsonPropertyName("auraJsonDir")]
    public string AuraJsonDir { get; set; } = AppConstants.ApplicationConfigDirectory;

    /// <summary>
    /// URLが有効な値であるかチェックする
    /// </summary>
    /// <param name="url">チェック対象のURL文字列</param>
    /// <returns>true: 有効, false: 無効</returns>
    private static bool IsValidUrl(string url)
    {
        return url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
               url.StartsWith("https://", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// 現在の設定データのクローンを作成します。
    /// </summary>
    /// <returns>クローンした設定データ</returns>
    public ConfigData Clone()
    {
        return new ConfigData
        {
            DiscordWebhookUrl = DiscordWebhookUrl,
            ToastNotification = ToastNotification,
            LogDir = LogDir,
            AuraJsonDir = AuraJsonDir,
        };
    }

    /// <summary>
    /// 2つのオブジェクトの全てのパブリックインスタンスプロパティの値が等しいかどうかを比較します。
    /// </summary>
    /// <typeparam name="T">比較するオブジェクトの型</typeparam>
    /// <param name="obj1">比較対象の1つ目のオブジェクト</param>
    /// <param name="obj2">比較対象の2つ目のオブジェクト</param>
    /// <returns>全てのプロパティが等しければtrue、そうでなければfalse</returns>
    public static bool AreEqual<T>(T obj1, T obj2)
    {
        if (obj1 == null || obj2 == null)
            return obj1 == null && obj2 == null;

        Type type = typeof(T);
        foreach (PropertyInfo prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            var val1 = prop.GetValue(obj1);
            var val2 = prop.GetValue(obj2);

            if (!Equals(val1, val2))
                return false;
        }

        return true;
    }
}

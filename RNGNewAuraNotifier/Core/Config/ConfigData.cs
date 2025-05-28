using System.Reflection;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

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

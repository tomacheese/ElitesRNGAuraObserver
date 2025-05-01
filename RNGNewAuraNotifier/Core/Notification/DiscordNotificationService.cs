using Discord;
using Discord.Webhook;
using RNGNewAuraNotifier.Core.Config;
using RNGNewAuraNotifier.Core.VRChat;
using System.Reflection;
using Color = Discord.Color;

namespace RNGNewAuraNotifier.Core.Notification;
internal class DiscordNotificationService
{
    /// <summary>
    /// DiscordのWebhookを使用してメッセージを送信する
    /// </summary>
    /// <param name="title">メッセージのタイトル</param>
    /// <param name="message">メッセージの内容</param>
    /// <param name="vrchatUser">VRChatユーザーの情報</param>
    public static async Task Notify(string title, string message, VRChatUser? vrchatUser)
    {
        var url = AppConfig.DiscordWebhookUrl;
        if (string.IsNullOrEmpty(url)) return;

        var version = Assembly.GetExecutingAssembly().GetName().Version ?? new Version(0, 0, 0);

        using var client = new DiscordWebhookClient(url);
        var embed = new EmbedBuilder
        {
            Title = title,
            Description = message,
            Footer = new EmbedFooterBuilder
            {
                Text = $"RNGNewAuraNotifier {version.Major}.{version.Minor}.{version.Build}",
            },
            Color = new Color(0x00, 0xFF, 0x00),
            Timestamp = DateTimeOffset.UtcNow,
        };
        if (vrchatUser != null)
        {
            embed.Author = new EmbedAuthorBuilder
            {
                Name = vrchatUser.UserName,
                Url = $"https://vrchat.com/home/user/{vrchatUser.UserId}",
            };
        }

        await client.SendMessageAsync(text: "", embeds: [embed.Build()]);
    }
}

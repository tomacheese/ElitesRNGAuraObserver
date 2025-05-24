using Discord;
using Discord.Webhook;
using RNGNewAuraNotifier.Core.Config;
using RNGNewAuraNotifier.Core.VRChat;
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
    public static async Task NotifyAsync(string title, string message, VRChatUser? vrchatUser)
    {
        var url = AppConfig.DiscordWebhookUrl;
        if (string.IsNullOrEmpty(url)) return;

        using var client = new DiscordWebhookClient(url);
        var embed = new EmbedBuilder
        {
            Title = title,
            Description = message,
            Footer = new EmbedFooterBuilder
            {
                Text = $"{AppConstants.AppName} {AppConstants.AppVersionString}",
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

        await client.SendMessageAsync(text: "", embeds: [embed.Build()]).ConfigureAwait(false);
    }

    /// <summary>
    /// DiscordのWebhookを使用してメッセージを送信する(Field形式)
    /// </summary>
    /// <param name="title">メッセージのタイトル</param>
    /// <param name="fields">メッセージの内容(Field)</param>
    /// <param name="vrchatUser">VRChatのユーザー情報</param>
    /// <returns></returns>
    public static async Task NotifyAsync(string title, List<(string Name, string Value, bool Inline)>? fields, VRChatUser? vrchatUser)
    {
        var url = AppConfig.DiscordWebhookUrl;
        if (string.IsNullOrEmpty(url)) return;

        using var client = new DiscordWebhookClient(url);
        var embed = new EmbedBuilder
        {
            Title = title,
            Footer = new EmbedFooterBuilder
            {
                Text = $"{AppConstants.AppName} {AppConstants.AppVersionString}",
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

        if (fields != null)
        {
            foreach ((var name, var value, var inline) in fields)
            {
                embed.AddField(name, value, inline);
            }
        }

        await client.SendMessageAsync(text: "", embeds: [embed.Build()]).ConfigureAwait(false);
    }
}

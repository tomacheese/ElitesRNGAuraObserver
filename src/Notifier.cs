using Microsoft.Toolkit.Uwp.Notifications;
using System.Threading.Tasks;
using Discord.Webhook;
using Discord;
using System;
using System.Reflection;

namespace RNGNewAuraNotifier
{
    /// <summary>
    /// 通知を管理するクラス
    /// </summary>
    public static class Notifier
    {
        /// <summary>
        /// Windowsのトースト通知を表示する
        /// </summary>
        /// <param name="title">通知のタイトル</param>
        /// <param name="message">通知のメッセージ</param>
        public static void ShowToast(string title, string message)
        {
            new ToastContentBuilder()
                .AddText(title)
                .AddText(message)
                .Show();
        }

        /// <summary>
        /// DiscordのWebhookを使用してメッセージを送信する
        /// </summary>
        /// <param name="title">メッセージのタイトル</param>
        /// <param name="message">メッセージの内容</param>
        /// <param name="vrchatUser">VRChatユーザーの情報</param>
        public static async Task SendDiscordWebhook(string title, string message, VRChatUser vrchatUser)
        {
            var url = AppConfig.DiscordWebhookUrl;
            if (url == null) return;

            var version = Assembly.GetExecutingAssembly().GetName().Version;

            using (var client = new DiscordWebhookClient(url))
            {
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

                await client.SendMessageAsync(text: "", embeds: new[] { embed.Build() });
            }
        }
    }
}

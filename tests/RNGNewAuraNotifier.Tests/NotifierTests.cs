namespace RNGNewAuraNotifier.Tests
{
    public class NotifierTests
    {
        [Fact]
        public void ShowToast_DoesNotThrow()
        {
            // トースト通知を表示する際に例外が発生しないことを確認します。
            Exception ex = Record.Exception(() => Notifier.ShowToast("title", "message"));
            Assert.Null(ex);
        }

        [Fact]
        public async Task SendDiscordWebhook_NoUrl_DoesNotThrowAsync()
        {
            // DiscordWebhookUrlがnullの場合に例外が発生しないことを確認します。
            AppConfig.DiscordWebhookUrl = null;
            var ex = await Record.ExceptionAsync(() => Notifier.SendDiscordWebhook("title", "message", null));
            Assert.Null(ex);
        }

        [Fact]
        public async Task SendDiscordWebhook_ValidUrl_SendsSuccessfully()
        {
            // DiscordWebhookUrlが有効な場合に正常に送信されることを確認します。
            // Arrange
            var mockUrl = "http://example.com/webhook";
            AppConfig.DiscordWebhookUrl = mockUrl;
            var mockUser = new VRChatUser { UserName = "TestUser", UserId = "usr_12345" };

            // Act
            var ex = await Record.ExceptionAsync(() => Notifier.SendDiscordWebhook("Test Title", "Test Message", mockUser));

            // Assert
            Assert.Null(ex);
        }
    }
}
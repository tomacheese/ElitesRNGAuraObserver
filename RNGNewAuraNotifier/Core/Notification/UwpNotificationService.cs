using Microsoft.Toolkit.Uwp.Notifications;

namespace RNGNewAuraNotifier.Core.Notification;

/// <summary>
/// Windowsのトースト通知を表示するサービス
/// </summary>

internal class UwpNotificationService
{
    /// <summary>
    /// Windowsのトースト通知を表示する
    /// </summary>
    /// <param name="title">通知のタイトル</param>
    /// <param name="message">通知のメッセージ</param>
    public static void Notify(string title, string message)
    {
        new ToastContentBuilder()
            .AddText(title)
            .AddText(message)
            .Show();
    }
}

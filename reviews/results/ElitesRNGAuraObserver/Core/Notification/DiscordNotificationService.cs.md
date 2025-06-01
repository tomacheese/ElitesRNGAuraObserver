# ElitesRNGAuraObserver/Core/Notification/DiscordNotificationService.cs レビュー

## 概要

`DiscordNotificationService.cs`はDiscord Webhookを使用してメッセージを送信するサービスクラスです。タイトル、ユーザー情報、メッセージ、フィールドなどを含むリッチな埋め込みメッセージを構築し、Discord Webhookを通じて送信する機能を提供します。

## 良い点

- 静的クラスとして実装され、シンプルなAPIを提供している
- リソース管理のためにusingステートメントを適切に使用している
- ConfigureAwait(false)を使用して非同期コンテキストの切り替えを最適化している
- NULLチェックを適切に行い、無効なURLや空のフィールドに対応している

## 改善点

### 1. インターフェースの導入

```csharp
// 現状
internal static class DiscordNotificationService

// 提案
// テスト容易性を向上させるためのインターフェース導入
public interface IDiscordNotificationService
{
    Task NotifyAsync(string discordWebhookUrl, string title, VRChatUser? vrchatUser, string? message = null, List<(string Name, string Value, bool Inline)>? fields = null);
}

internal class DiscordNotificationService : IDiscordNotificationService
{
    public async Task NotifyAsync(string discordWebhookUrl, string title, VRChatUser? vrchatUser, string? message = null, List<(string Name, string Value, bool Inline)>? fields = null)
    {
        // 実装...
    }
}
```

### 2. URL検証の強化

```csharp
// 現状
var url = discordWebhookUrl;
if (string.IsNullOrEmpty(url)) return;

// 提案
// URLの形式検証を追加
if (string.IsNullOrEmpty(discordWebhookUrl)) return;

if (!Uri.TryCreate(discordWebhookUrl, UriKind.Absolute, out var uri) ||
    !uri.Host.EndsWith("discord.com", StringComparison.OrdinalIgnoreCase) ||
    !uri.PathAndQuery.Contains("/api/webhooks/"))
{
    throw new ArgumentException("Invalid Discord webhook URL format", nameof(discordWebhookUrl));
}

var url = discordWebhookUrl;
```

### 3. 例外処理の強化

```csharp
// 現状
// 例外処理が不足している

// 提案
// 適切な例外処理の追加
public static async Task NotifyAsync(string discordWebhookUrl, string title, VRChatUser? vrchatUser, string? message = null, List<(string Name, string Value, bool Inline)>? fields = null)
{
    if (string.IsNullOrEmpty(discordWebhookUrl)) return;

    try
    {
        using var client = new DiscordWebhookClient(discordWebhookUrl);
        // ... 埋め込みメッセージの構築 ...

        await client.SendMessageAsync(text: string.Empty, embeds: [embed.Build()]).ConfigureAwait(false);
    }
    catch (Exception ex)
    {
        // 例外をログに記録
        Console.WriteLine($"Discord notification failed: {ex.Message}");
        // 呼び出し元に例外を再スローすることも検討
        throw new DiscordNotificationException("Failed to send Discord notification", ex);
    }
}

// カスタム例外クラス
public class DiscordNotificationException : Exception
{
    public DiscordNotificationException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
```

### 4. 色の定数化

```csharp
// 現状
Color = new Color(0x00, 0xFF, 0x00),

// 提案
// 色を定数として定義
private static readonly Color SuccessColor = new(0x00, 0xFF, 0x00); // 緑色

// メソッド内での使用
Color = SuccessColor,
```

### 5. フィールド名のバリデーション

```csharp
// 現状
// フィールド名のバリデーションがない

// 提案
// フィールド名と値のバリデーションを追加
if (fields != null)
{
    foreach ((var name, var value, var inline) in fields)
    {
        if (string.IsNullOrEmpty(name))
        {
            Console.WriteLine("Field name cannot be empty, skipping field");
            continue;
        }

        // Discord APIの制限: 名前は256文字、値は1024文字まで
        var trimmedName = name.Length > 256 ? name[..253] + "..." : name;
        var trimmedValue = value.Length > 1024 ? value[..1021] + "..." : value;

        embed.AddField(trimmedName, trimmedValue, inline);
    }
}
```

## セキュリティ上の懸念

1. **Webhook URL の取り扱い** - Webhook URLは機密情報であり、適切に保護されるべきです。現在の実装では、URLは引数として渡されるだけで、暗号化や保護の仕組みがありません。
2. **入力検証** - ユーザー入力（特にメッセージやフィールド）に対する検証が不足しているため、悪意のある入力がDiscordに送信される可能性があります。

## パフォーマンス上の懸念

特に重大なパフォーマンス上の問題は見当たりません。

## 命名規則

命名規則は適切に守られており、C#の標準的な命名規則（PascalCase、camelCase）に従っています。

## まとめ

全体的に機能的なサービスクラスですが、インターフェースの導入、URL検証の強化、例外処理の改善、フィールド名のバリデーションなどにより、コードの保守性、テスト容易性、および堅牢性を向上させることができます。特に、Webhook URLの検証と例外処理の追加は、アプリケーションの安定性を高めるために重要な改善点です。

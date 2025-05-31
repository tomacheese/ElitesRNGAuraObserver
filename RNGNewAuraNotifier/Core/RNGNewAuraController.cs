using RNGNewAuraNotifier.Core.Aura;
using RNGNewAuraNotifier.Core.Config;
using RNGNewAuraNotifier.Core.Notification;
using RNGNewAuraNotifier.Core.VRChat;

namespace RNGNewAuraNotifier.Core;

/// <summary>
/// VRChatのログを監視し新しいAuraの取得を検出して通知するコントローラークラス
/// </summary>
/// <remarks>
/// コンストラクタ
/// </remarks>
/// <param name="configData">ログディレクトリのパス</param>
/// <remarks>ログディレクトリにnullまたは空白を指定した場合は、デフォルトのVRChatログディレクトリを使用する</remarks>
internal class RNGNewAuraController(ConfigData configData) : IDisposable
{
    /// <summary>
    /// ログウォッチャー
    /// </summary>
    private readonly LogWatcher _logWatcher = new("output_log_*.txt");

    /// <summary>
    /// ログインしているVRChatユーザーの情報
    /// </summary>
    private VRChatUser? _vrchatUser;

    /// <summary>
    /// ログ監視を開始する
    /// </summary>
    public void Start()
    {
        Console.WriteLine("RNGNewAuraController.Start");
        new AuthenticatedDetectionService(_logWatcher).OnDetected += OnAuthenticatedUser;
        new NewAuraDetectionService(_logWatcher).OnDetected += OnNewAuraDetected;
        _logWatcher.Start();
    }

    /// <summary>
    /// ログ監視を破棄する
    /// </summary>
    public void Dispose()
    {
        Console.WriteLine("RNGNewAuraController.Dispose");
        _logWatcher.Stop();
        _logWatcher.Dispose();
    }

    /// <summary>
    /// 監視しているログファイルのパスを取得する
    /// </summary>
    /// <returns>監視しているログファイルのパス</returns>
    public string GetLastReadFilePath() => _logWatcher.GetLastReadFilePath();

    /// <summary>
    /// VRChatユーザーのログイン完了行を検出したときのハンドラ
    /// </summary>
    /// <param name="user">VRChatユーザー</param>
    /// <param name="isFirstReading">初回読み込みかどうか</param>
    private void OnAuthenticatedUser(VRChatUser user, bool isFirstReading)
    {
        Console.WriteLine($"Authenticated User: {user.UserName} ({user.UserId})");
        _vrchatUser = user;
    }

    /// <summary>
    /// 新しいAuraを取得した行を検出したときのハンドラ
    /// </summary>
    /// <param name="aura">取得したAura</param>
    /// <param name="isFirstReading">初回読み込みかどうか</param>
    private void OnNewAuraDetected(Aura.Aura aura, bool isFirstReading)
    {
        Console.WriteLine($"New Aura: {aura.Name} (#{aura.Id}) - {isFirstReading}");

        // 初回読み込み、またはTier5のAuraは通知しない
        if (isFirstReading || aura.Tier == 5)
        {
            return;
        }

        if (configData.ToastNotification)
            UwpNotificationService.Notify("Unlocked New Aura!", $"{aura.GetNameText()}\n{aura.GetRarityString()}");

        Task.Run(async () =>
        {
            try
            {
                // Aura名が取得できなかった場合は、"_Unknown_"を表示する
                var auraName = string.IsNullOrEmpty(aura.GetNameText()) ? $"_Unknown_" : $"`{aura.GetNameText()}`";
                var auraRarity = $"`{aura.GetRarityString()}`";
                var fields = new List<(string Name, string Value, bool Inline)>
                {
                    ("Aura Name", auraName, true),
                    ("Rarity", auraRarity, true),
                };

                await DiscordNotificationService.NotifyAsync(
                    discordWebhookUrl: configData.DiscordWebhookUrl,
                    title: "**Unlocked New Aura!**",
                    vrchatUser: _vrchatUser,
                    fields: fields
                ).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] DiscordWebhook: {ex.Message}");
            }
        }).Wait();
    }
}

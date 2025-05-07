using RNGNewAuraNotifier.Core.Aura;
using RNGNewAuraNotifier.Core.Notification;
using RNGNewAuraNotifier.Core.VRChat;

namespace RNGNewAuraNotifier.Core;
internal class RNGNewAuraController
{
    /// <summary>
    /// 監視対象のログディレクトリパス
    /// </summary>
    private readonly string _logDir;

    /// <summary>
    /// ログウォッチャー
    /// </summary>
    private readonly LogWatcher _logWatcher;

    /// <summary>
    /// ログインしているVRChatユーザーの情報
    /// </summary>
    private VRChatUser? _vrchatUser;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="logDirectory">ログディレクトリのパス</param>
    /// <remarks>ログディレクトリにnullまたは空白を指定した場合は、デフォルトのVRChatログディレクトリを使用する</remarks>
    public RNGNewAuraController(string? logDirectory)
    {

        // ログディレクトリが指定されていない場合は、デフォルトのVRChatログディレクトリを使用する
        var defaultLogDir = AppConstant.VRChatDefaultLogDirectory;
        _logDir = logDirectory ?? defaultLogDir;
        if (string.IsNullOrEmpty(_logDir))
        {
            _logDir = defaultLogDir;
        }

        _logWatcher = new LogWatcher(_logDir, "output_log_*.txt");
    }

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
    /// 監視しているログディレクトリのパスを取得する
    /// </summary>
    /// <returns>監視しているログディレクトリのパス</returns>
    public string GetLogDirectory() => _logDir;

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
        if (isFirstReading || aura.Tier == 5)
        {
            return;
        }
        UwpNotificationService.Notify("Unlocked New Aura!", $"{aura.GetNameText()}\n{aura.GetRarityString()}");
        Task.Run(async () =>
        {
            try
            {
                await DiscordNotificationService.Notify("Unlocked New Aura!", $"**Aura Name: **`{aura.GetNameText()}`\n**Rarity: ** `{aura.GetRarityString()}`", _vrchatUser);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] DiscordWebhook: {ex.Message}");
            }
        }).Wait();
    }
}
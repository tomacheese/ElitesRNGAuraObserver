using System.Text;

namespace RNGNewAuraNotifier.Core.VRChat;

/// <summary>
/// VRChatのログファイルを監視するクラス
/// </summary>
/// <param name="logDirectory">ログディレクトリのパス</param>
/// <param name="logFileFilter">ログファイルのフィルタ</param>
internal class LogWatcher(string logFileFilter) : IDisposable
{
    /// <summary>
    /// 新規ログ行を検出したときに発生するイベント
    /// </summary>
    public event Action<string, bool> OnNewLogLine = (arg1, arg2) => { };

    /// <summary>
    /// キャンセル用のトークンソース
    /// </summary>
    private readonly CancellationTokenSource _cts = new();

    /// <summary>
    /// ログファイルのパターンフィルタ
    /// </summary>
    private readonly string _logFileFilter = logFileFilter;

    /// <summary>
    /// 最後に読み取ったファイルのパス
    /// </summary>
    private string _lastReadFilePath = GetNewestLogFile(logFileFilter) ?? string.Empty;

    /// <summary>
    /// 最後に読み取った位置
    /// </summary>
    private long _lastPosition = 0;

    /// <summary>
    /// ログファイルの監視を開始する
    /// </summary>
    public void Start()
    {
        Console.WriteLine($"LogWatcher.Start: {_lastReadFilePath}");

        // 監視対象の最新ログファイルが存在する場合は、最初に処理する
        if (!string.IsNullOrEmpty(_lastReadFilePath))
        {
            ReadNewLine(_lastReadFilePath);
        }

        Task.Run(() => MonitorLoopAsync(_cts.Token), _cts.Token)
            .ContinueWith(t =>
            {
                if (t.IsFaulted)
                {
                    Console.WriteLine($"LogWatcher error: {t.Exception?.GetBaseException().Message}");
                }
            }, TaskContinuationOptions.OnlyOnFaulted);
    }

    /// <summary>
    /// 監視を停止する
    /// </summary>
    public void Stop() => _cts.Cancel();

    /// <summary>
    /// 監視を破棄する
    /// </summary>
    public void Dispose() => _cts.Dispose();

    /// <summary>
    /// 最後に読み取ったファイルのパスを取得する
    /// </summary>
    /// <returns>最後に読み取ったファイルのパス</returns>
    public string GetLastReadFilePath() => _lastReadFilePath;

    /// <summary>
    /// 最後に読み取った位置を取得する
    /// </summary>
    /// <returns>>最後に読み取った位置</returns>
    public long GetLastPosition() => _lastPosition;

    /// <summary>
    /// 1秒おきにログファイルを監視する
    /// </summary>
    /// <param name="token">キャンセルトークン</param>
    /// <returns>タスク</returns>
    private async Task MonitorLoopAsync(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            // 監視対象の最新ログファイルを取得する
            var newestLogFile = GetNewestLogFile(_logFileFilter);
            if (newestLogFile == null)
            {
                Console.WriteLine($"No log file found in {AppConstants.VRChatLogDirectory}");
                await Task.Delay(1000, token).ConfigureAwait(false);
                continue;
            }

            // 最新のログファイルが変更された場合は、読み込み位置をリセットする
            if (_lastReadFilePath != newestLogFile || _lastPosition == 0)
            {
                _lastPosition = 0;
            }

            // 最新のログファイルを読み込む
            ReadNewLine(newestLogFile);
            await Task.Delay(1000, token).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// 指定されたログファイルを読み込む
    /// </summary>
    /// <param name="path">ログファイルのパス</param>
    private void ReadNewLine(string path)
    {
        Console.WriteLine($"ReadNewLine: {path} ({_lastPosition})");
        try
        {
            using var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            if (stream.Length == 0)
            {
                Console.WriteLine($"File is empty: {path}");
                return;
            }

            if (_lastReadFilePath != path || stream.Length < _lastPosition)
            {
                Console.WriteLine($"File changed: {_lastReadFilePath} -> {path} | {_lastPosition} -> {stream.Length}");
                _lastPosition = 0;
            }

            // 最初の読み込みかどうかは、最終読み込み位置がゼロのときとする
            var isFirstReading = _lastPosition == 0;

            stream.Seek(_lastPosition, SeekOrigin.Begin);

            using var reader = new StreamReader(stream, Encoding.UTF8);
            string? line;
            while ((line = reader.ReadLine()) != null)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                Console.WriteLine($"Log line: {line}");
                try
                {
                    OnNewLogLine.Invoke(line, isFirstReading);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing log line: {ex.Message}");
                }

                _lastReadFilePath = path;
                _lastPosition = stream.Position;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to read log file: {ex.Message}");
        }
    }

    /// <summary>
    /// 指定されたディレクトリ内の最新のログファイルを取得する
    /// </summary>
    /// <param name="logFileFilter">ログファイルのフィルタ</param>
    /// <returns>最新のログファイルのパス</returns>
    private static string? GetNewestLogFile(string logFileFilter)
    {
        var files = Directory.GetFiles(AppConstants.VRChatLogDirectory, logFileFilter);
        return files.Length == 0 ? null : files.OrderByDescending(static f => File.GetLastWriteTime(f)).FirstOrDefault();
    }
}

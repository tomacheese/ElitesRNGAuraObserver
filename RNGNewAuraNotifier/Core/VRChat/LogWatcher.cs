using System.Text;

namespace RNGNewAuraNotifier.Core.VRChat;
internal class LogWatcher
{
    /// <summary>
    /// 新規ログ行を検出したときに発生するイベント
    /// </summary>
    public event Action<string, bool> OnNewLogLine = delegate { };

    /// <summary>
    /// キャンセル用のトークンソース
    /// </summary>
    private readonly CancellationTokenSource _cts = new();

    /// <summary>
    /// ログディレクトリ
    /// </summary>
    private readonly string _logDirectory;

    /// <summary>
    /// ログファイルのパターンフィルタ
    /// </summary>
    private readonly string _logFileFilter;

    /// <summary>
    /// 最後に読み取ったファイルのパス
    /// </summary>
    private string _lastReadFilePath = string.Empty;

    /// <summary>
    /// 最後に読み取った位置
    /// </summary>
    private long _lastPosition = 0;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="logDirectory">ログディレクトリのパス</param>
    /// <param name="logFileFilter">ログファイルのフィルタ</param>
    public LogWatcher(string logDirectory, string logFileFilter)
    {
        _logDirectory = logDirectory;
        _logFileFilter = logFileFilter;

        // コンストラクタ生成時に最新のログファイルを取得する
        _lastReadFilePath = GetNewestLogFile(logDirectory, logFileFilter) ?? string.Empty;
    }

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

        Task.Run(() => MonitorLoop(_cts.Token), _cts.Token)
            .ContinueWith(t =>
            {
                if (t.IsFaulted)
                {
                    Console.WriteLine($"LogWatcher error: {t.Exception?.GetBaseException().Message}");
                }
            }, TaskContinuationOptions.OnlyOnFaulted);
    }

    public async Task Stop()
    {
        _cts.Cancel();
        if (_monitorTask != null)
        {
            await _monitorTask;
        }
    }

    public void Dispose()
    {
        Stop().GetAwaiter().GetResult();
        _cts.Dispose();
    }
    public string GetLastReadFilePath() => _lastReadFilePath;

    public long GetLastPosition() => _lastPosition;

    private async Task MonitorLoop(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            // 監視対象の最新ログファイルを取得する
            var newestLogFile = GetNewestLogFile(_logDirectory, _logFileFilter);
            if (newestLogFile == null)
            {
                Console.WriteLine($"No log file found in {_logDirectory}");
                await Task.Delay(1000, token);
                continue;
            }
            // 最新のログファイルが変更された場合は、読み込み位置をリセットする
            if (_lastReadFilePath != newestLogFile || _lastPosition == 0)
            {
                _lastPosition = 0;
            }

            // 最新のログファイルを読み込む
            ReadNewLine(newestLogFile);
            await Task.Delay(1000, token);
        }
    }

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
    /// <param name="logDirectory">ログディレクトリのパス</param>
    /// <param name="logFileFilter">ログファイルのフィルタ</param>
    /// <returns>最新のログファイルのパス</returns>
    private static string? GetNewestLogFile(string logDirectory, string logFileFilter)
    {
        var files = Directory.GetFiles(logDirectory, logFileFilter);
        if (files.Length == 0)
        {
            return null;
        }
        return files.OrderByDescending(static f => File.GetLastWriteTime(f)).FirstOrDefault();
    }
}
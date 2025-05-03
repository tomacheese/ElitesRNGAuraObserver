using System.Text;

namespace RNGNewAuraNotifier.Core.VRChat;
internal class LogWatcher
{
    /// <summary>
    /// 新規ログ行を検出したときに発生するイベント
    /// </summary>
    public event Action<string, bool> OnNewLogLine = delegate { };

    /// <summary>
    /// ファイルシステムウォッチャー
    /// </summary>
    private readonly FileSystemWatcher _fsw;

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
        _fsw = new FileSystemWatcher(logDirectory, logFileFilter)
        {
            NotifyFilter = NotifyFilters.LastWrite,
            EnableRaisingEvents = false // インスタンス生成時はイベントを無効にする
        };
        // ファイルが作成・変更されたときにイベントを発生させる
        _fsw.Created += (_, e) => ReadNewLine(e.FullPath);
        _fsw.Changed += (_, e) => ReadNewLine(e.FullPath);

        // コンストラクタ生成時に最新のログファイルを取得する
        _lastReadFilePath = GetNewestLogFile(logDirectory, logFileFilter) ?? string.Empty;
    }

    /// <summary>
    /// ログファイルの監視を開始する
    /// </summary>
    public void Start()
    {
        Console.WriteLine($"LogWatcher.Start: {_fsw.Path} | {_fsw.Filter} | {_lastReadFilePath}");

        _fsw.EnableRaisingEvents = true;

        // 監視対象の最新ログファイルが存在する場合は、最初に処理する
        if (!string.IsNullOrEmpty(_lastReadFilePath))
        {
            ReadNewLine(_lastReadFilePath);
        }
    }

    public void Stop() => _fsw.EnableRaisingEvents = false;
    public void Dispose() => _fsw.Dispose();

    public string GetLastReadFilePath() => _lastReadFilePath;

    public long GetLastPosition() => _lastPosition;

    public static string GetDefaultVRChatLogDirectory()
    {
        return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "AppData", "LocalLow", "VRChat", "VRChat");
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

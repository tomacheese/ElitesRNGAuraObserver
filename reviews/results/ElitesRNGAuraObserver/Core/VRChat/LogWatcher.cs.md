# ElitesRNGAuraObserver/Core/VRChat/LogWatcher.cs レビュー

## 概要

`LogWatcher.cs`はVRChatのログファイルを監視し、新しいログ行を検出するクラスです。ファイルシステムを利用して最新のログファイルを定期的に監視し、新しいログ行が追加されるとイベントを発行します。

## 良い点

- ファイルシステムの共有アクセスを考慮し、FileShareオプションを適切に設定している
- タスクベースの非同期処理を使用して、UIスレッドをブロックしないよう配慮している
- キャンセルトークンを使用して、適切にリソース管理している
- ログファイルの変更検出と位置の追跡が適切に実装されている
- 各メソッドに適切なコメントが記述されている

## 改善点

### 1. インターフェースの導入

```csharp
// 現状
internal class LogWatcher(string logFileFilter) : IDisposable

// 提案
// テスト容易性を向上させるためのインターフェース導入
public interface ILogWatcher : IDisposable
{
    event Action<string, bool> OnNewLogLine;
    void Start();
    void Stop();
    string GetLastReadFilePath();
    long GetLastPosition();
}

internal class LogWatcher : ILogWatcher
{
    private readonly string _logFileFilter;

    public LogWatcher(string logFileFilter)
    {
        _logFileFilter = logFileFilter ?? throw new ArgumentNullException(nameof(logFileFilter));
    }

    // 実装...
}
```

### 2. 設定の依存性注入

```csharp
// 現状
ConfigData configData = AppConfig.Instance;

// 提案
// 設定の依存性注入
private readonly ConfigData _configData;

public LogWatcher(string logFileFilter, ConfigData configData)
{
    _logFileFilter = logFileFilter ?? throw new ArgumentNullException(nameof(logFileFilter));
    _configData = configData ?? throw new ArgumentNullException(nameof(configData));
}
```

### 3. イベントハンドラの管理改善

```csharp
// 現状
public event Action<string, bool> OnNewLogLine = (arg1, arg2) => { };

// 提案
// nullチェックによるイベント発火
public event Action<string, bool>? OnNewLogLine;

private void RaiseOnNewLogLine(string line, bool isFirstReading)
{
    OnNewLogLine?.Invoke(line, isFirstReading);
}

// 使用箇所
RaiseOnNewLogLine(line, isFirstReading);
```

### 4. エラーロギングの改善

```csharp
// 現状
Console.WriteLine($"Failed to read log file: {ex.Message}");

// 提案
// 構造化ログの使用と詳細な例外情報
private static readonly ILogger _logger = LoggerFactory.Create(builder =>
    builder.AddConsole()).CreateLogger<LogWatcher>();

try
{
    // ...
}
catch (IOException ex)
{
    _logger.LogError(ex, "Failed to read log file {FilePath}: {ErrorMessage}", path, ex.Message);
}
catch (UnauthorizedAccessException ex)
{
    _logger.LogError(ex, "Access denied to log file {FilePath}: {ErrorMessage}", path, ex.Message);
}
catch (Exception ex)
{
    _logger.LogError(ex, "Unexpected error reading log file {FilePath}: {ErrorMessage}", path, ex.Message);
}
```

### 5. ファイル監視の最適化

```csharp
// 現状
// 定期的なポーリングによるファイル監視

// 提案
// FileSystemWatcherを使用したイベント駆動型のファイル監視
private FileSystemWatcher? _fileWatcher;

public void Start()
{
    Console.WriteLine($"LogWatcher.Start: {_lastReadFilePath}");

    // 監視対象の最新ログファイルが存在する場合は、最初に処理する
    if (!string.IsNullOrEmpty(_lastReadFilePath))
    {
        ReadNewLine(_lastReadFilePath);
    }

    // ファイルシステムウォッチャーの設定
    _fileWatcher = new FileSystemWatcher(_configData.LogDir)
    {
        Filter = _logFileFilter,
        NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.CreationTime,
        EnableRaisingEvents = true
    };

    _fileWatcher.Changed += OnFileChanged;
    _fileWatcher.Created += OnFileChanged;

    // バックグラウンドで定期的にチェックも行う（イベントが漏れる可能性に備える）
    Task.Run(() => MonitorLoopAsync(_cts.Token), _cts.Token)
        .ContinueWith(t =>
        {
            if (t.IsFaulted)
            {
                _logger.LogError(t.Exception, "LogWatcher monitoring loop failed");
            }
        }, TaskContinuationOptions.OnlyOnFaulted);
}

private void OnFileChanged(object sender, FileSystemEventArgs e)
{
    try
    {
        ReadNewLine(e.FullPath);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error processing file change: {FilePath}", e.FullPath);
    }
}
```

## セキュリティ上の懸念

1. **ファイルシステムアクセス** - ユーザーが指定したディレクトリのファイルにアクセスするため、不正なパスが指定された場合のリスクがあります。パスの検証を強化すべきです。

## パフォーマンス上の懸念

1. **定期的なファイル読み込み** - 1秒ごとにファイルをポーリングしているため、CPU使用率が高くなる可能性があります。FileSystemWatcherを使用してイベント駆動型のアプローチに変更することを検討すべきです。
2. **大きなファイル処理** - 大きなログファイルを扱う場合に、メモリ使用量が増加する可能性があります。バッファサイズやチャンク読み込みの最適化を検討すべきです。

## 命名規則

命名規則は適切に守られており、C#の標準的な命名規則（PascalCase、camelCase）に従っています。

## まとめ

全体的に機能的なログ監視クラスですが、インターフェースの導入、設定の依存性注入、イベントハンドラの管理改善、構造化ログの使用、およびFileSystemWatcherによるイベント駆動型アーキテクチャの採用により、コードの保守性、テスト容易性、およびパフォーマンスを向上させることができます。特に、FileSystemWatcherの導入とエラーロギングの強化は、アプリケーションの安定性とパフォーマンスを高めるために重要な改善点です。

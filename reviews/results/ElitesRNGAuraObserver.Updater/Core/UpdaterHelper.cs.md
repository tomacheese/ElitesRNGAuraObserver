# ElitesRNGAuraObserver.Updater/Core/UpdaterHelper.cs レビュー

## 概要

このファイルは、アップデート処理をサポートするユーティリティ機能を提供する静的ヘルパークラスです。プロセスの終了、ZIPファイルの展開、ファイルのチェックサム検証といった機能を実装しています。

## 良い点

- XMLドキュメントコメントが適切に記述されている
- プロセス終了時には、まず正常終了を試み、その後強制終了するという段階的なアプローチを採用している
- ZIPファイルの展開時にディレクトリトラバーサル攻撃を防止するためのパスチェックが実装されている
- ファイルのダイジェスト検証がSHA-256ハッシュを使用して適切に実装されている
- 各処理でのエラーハンドリングが考慮されている

## 改善点

### 1. KillProcesses メソッドの強化

```csharp
// 現状
Process[] processes = Process.GetProcessesByName(processName);

// 改善案
// プロセスのパスも確認して、同名の他プロセスを誤って終了させないようにする
public static void KillProcesses(string processName, string? executablePath = null)
{
    Process[] processes = Process.GetProcessesByName(processName);
    foreach (Process proc in processes)
    {
        try
        {
            // 指定されたパスがあれば、プロセスのパスを確認
            if (executablePath != null)
            {
                try
                {
                    string procPath = proc.MainModule?.FileName ?? string.Empty;
                    if (!string.Equals(procPath, executablePath, StringComparison.OrdinalIgnoreCase))
                    {
                        Console.WriteLine($"Skipping process Id={proc.Id} with different path: {procPath}");
                        continue;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Could not verify process path for Id={proc.Id}: {ex.Message}");
                    // プロセスへのアクセス権がない場合など、パスの取得に失敗することがある
                    // この場合は安全のため、終了処理を続行しない
                    continue;
                }
            }

            // 以降は既存のコード
        }
        // 以降は既存のコード
    }
}
```

### 2. ExtractZipToTarget メソッドのエラーハンドリング強化

```csharp
// 現状
entry.ExtractToFile(fullPath, overwrite: true);

// 改善案
// より詳細なエラーハンドリングと進捗報告
try
{
    entry.ExtractToFile(fullPath, overwrite: true);
    Console.WriteLine($"Extracted: {entry.FullName}");
}
catch (IOException ex)
{
    // ファイルが使用中の場合など
    Console.Error.WriteLine($"Error extracting {entry.FullName}: {ex.Message}");
    throw new IOException($"Failed to extract file {entry.FullName}. The file may be in use.", ex);
}
catch (UnauthorizedAccessException ex)
{
    // アクセス権限がない場合
    Console.Error.WriteLine($"Access denied extracting {entry.FullName}: {ex.Message}");
    throw new UnauthorizedAccessException($"Access denied when extracting {entry.FullName}. Check permissions.", ex);
}
```

### 3. ファイルロック対策

```csharp
// 改善案
// ファイルが使用中の場合のリトライロジック
private static void ExtractWithRetry(ZipArchiveEntry entry, string destinationPath, int maxRetries = 3)
{
    int attempt = 0;
    while (true)
    {
        try
        {
            attempt++;
            entry.ExtractToFile(destinationPath, overwrite: true);
            return;
        }
        catch (IOException) when (attempt < maxRetries)
        {
            Console.WriteLine($"Failed to extract {entry.FullName}, retrying ({attempt}/{maxRetries})...");
            Thread.Sleep(500 * attempt); // 指数バックオフ
        }
    }
}
```

### 4. VerifyDigest メソッドの強化

```csharp
// 現状
var actualDigest = $"sha256:{Convert.ToHexStringLower(hash)}";
return string.Equals(actualDigest, expectedDigest, StringComparison.OrdinalIgnoreCase);

// 改善案
// ダイジェスト形式のパース
public static bool VerifyDigest(string filePath, string expectedDigest)
{
    if (string.IsNullOrEmpty(expectedDigest))
    {
        throw new ArgumentException("Expected digest cannot be null or empty.", nameof(expectedDigest));
    }

    if (!File.Exists(filePath))
    {
        throw new FileNotFoundException($"File not found: {filePath}");
    }

    // ダイジェスト形式のパース (例: "sha256:617d6861ded8...")
    string[] parts = expectedDigest.Split(':', 2);
    if (parts.Length != 2 || !string.Equals(parts[0], "sha256", StringComparison.OrdinalIgnoreCase))
    {
        throw new ArgumentException($"Unsupported digest format: {expectedDigest}. Only SHA-256 is supported.", nameof(expectedDigest));
    }

    string expectedHash = parts[1];

    using var sha256 = System.Security.Cryptography.SHA256.Create();
    using FileStream stream = File.OpenRead(filePath);
    var hash = sha256.ComputeHash(stream);
    var actualHash = Convert.ToHexStringLower(hash);

    bool isMatch = string.Equals(actualHash, expectedHash, StringComparison.OrdinalIgnoreCase);
    Console.WriteLine($"Digest verification: {(isMatch ? "Succeeded" : "Failed")}");
    Console.WriteLine($"Expected: {expectedHash}");
    Console.WriteLine($"Actual  : {actualHash}");

    return isMatch;
}
```

### 5. 非同期処理の導入

```csharp
// 改善案
// 特にファイルI/Oを伴う処理は非同期にすることで、UIスレッドのブロックを防ぐ
public static async Task<bool> VerifyDigestAsync(string filePath, string expectedDigest, CancellationToken cancellationToken = default)
{
    if (string.IsNullOrEmpty(expectedDigest))
    {
        throw new ArgumentException("Expected digest cannot be null or empty.", nameof(expectedDigest));
    }

    if (!File.Exists(filePath))
    {
        throw new FileNotFoundException($"File not found: {filePath}");
    }

    using var sha256 = System.Security.Cryptography.SHA256.Create();
    using FileStream stream = File.OpenRead(filePath);
    var hash = await Task.Run(() => sha256.ComputeHash(stream), cancellationToken).ConfigureAwait(false);
    var actualDigest = $"sha256:{Convert.ToHexStringLower(hash)}";
    return string.Equals(actualDigest, expectedDigest, StringComparison.OrdinalIgnoreCase);
}
```

## セキュリティリスク

1. ディレクトリトラバーサル攻撃対策は実装されていますが、ZIPファイルの展開時に悪意のあるファイル名（例：COM1など特殊なデバイス名）に対する追加のチェックが必要かもしれません。

2. SHA-256ハッシュを使用したダイジェスト検証は適切ですが、将来的にはより強力なハッシュアルゴリズム（SHA-384、SHA-512など）もサポートすることを検討すべきです。

3. プロセス終了のセキュリティ：現在の実装では、同じ名前のすべてのプロセスを終了させるため、同名の他のアプリケーションも影響を受ける可能性があります。

## パフォーマンス上の懸念

1. 大きなファイルのハッシュ計算やZIPファイルの展開は、特にメインスレッドで実行された場合、UIの応答性に影響を与える可能性があります。非同期処理の導入を検討すべきです。

2. メモリ使用量：ZIPファイルの展開時には、特に大きなファイルやエントリが多数あるZIPファイルの場合、メモリ使用量が一時的に増加する可能性があります。バッファサイズの制御やストリーミング展開の検討が必要かもしれません。

## 全体評価

全体として、このヘルパークラスはアップデート処理に必要な基本的な機能を適切に提供しています。セキュリティ対策も一定レベルで実装されており、特にZIPファイル展開時のディレクトリトラバーサル攻撃への対策は評価できます。ただし、プロセス終了の安全性向上、ファイルロック対策、非同期処理の導入などの改善点があります。また、エラーメッセージの改善とより詳細なログ記録を追加することで、トラブルシューティングが容易になるでしょう。

# ElitesRNGAuraObserver/Core/Config/RegistryManager.cs レビュー

## 概要

`RegistryManager.cs`はWindowsのスタートアップにアプリケーションを登録/解除するための機能を提供する管理クラスです。Windowsレジストリを操作して、アプリケーションがWindowsの起動時に自動的に実行されるようにします。

## 良い点

- レジストリ操作に関するロジックが一箇所に集約されている
- 現在のアプリケーションパスとレジストリに登録されているパスの比較チェックが実装されている
- 必要に応じて自動的にレジストリを更新する機能がある
- レジストリの読み取り専用/書き込み可能なアクセスが適切に区別されている

## 改善点

### 1. インターフェースの導入

```csharp
// 現状
internal class RegistryManager

// 提案
// テスト容易性を向上させるためのインターフェース導入
public interface IStartupManager
{
    void SetStartup(bool enableStartup);
    void EnsureStartupRegistration();
    bool IsRegisteredStartup();
}

internal class RegistryManager : IStartupManager
{
    // 実装...
}
```

### 2. 例外処理の強化

```csharp
// 現状
// 例外処理がない

// 提案
// 例外処理の追加
public static void SetStartup(bool enableStartup)
{
    try
    {
        using RegistryKey? key = Registry.CurrentUser.OpenSubKey(StartupKeyPath, true);
        if (key == null)
        {
            throw new InvalidOperationException($"Failed to open registry key: {StartupKeyPath}");
        }

        if (enableStartup)
        {
            var exePath = Application.ExecutablePath;
            key.SetValue(AppConstants.AssemblyName, "\"" + exePath + "\"");
            Console.WriteLine($"Added application to startup: {exePath}");
        }
        else
        {
            key.DeleteValue(AppConstants.AssemblyName, false);
            Console.WriteLine("Removed application from startup");
        }
    }
    catch (UnauthorizedAccessException ex)
    {
        Console.WriteLine($"Access denied when modifying registry: {ex.Message}");
        throw new InvalidOperationException("Failed to modify startup registry due to access rights. Try running as administrator.", ex);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error modifying startup registry: {ex.Message}");
        throw new InvalidOperationException("Failed to modify startup registry settings.", ex);
    }
}
```

### 3. ロギングの改善

```csharp
// 現状
// ロギングがない

// 提案
// 構造化ログの使用
private static readonly ILogger _logger = LoggerFactory.Create(builder =>
    builder.AddConsole()).CreateLogger(typeof(RegistryManager));

public static void EnsureStartupRegistration()
{
    try
    {
        using RegistryKey? key = Registry.CurrentUser.OpenSubKey(StartupKeyPath, true);
        if (key == null)
        {
            _logger.LogWarning("Failed to open registry key: {StartupKeyPath}", StartupKeyPath);
            return;
        }

        var value = key.GetValue(AppConstants.AssemblyName);
        var currentExePath = $"\"{Application.ExecutablePath}\"";

        if (value == null)
        {
            _logger.LogInformation("Application is not registered in startup");
            return;
        }

        if (value.ToString() != currentExePath)
        {
            _logger.LogInformation("Updating startup registry entry from {OldPath} to {NewPath}",
                value, currentExePath);
            key.SetValue(AppConstants.AssemblyName, currentExePath);
        }
        else
        {
            _logger.LogDebug("Startup registry entry is up to date");
        }
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Failed to ensure startup registration");
    }
}
```

### 4. 管理者権限の確認

```csharp
// 現状
// 管理者権限の確認がない

// 提案
// 管理者権限の確認
public static bool IsAdministrator()
{
    var identity = System.Security.Principal.WindowsIdentity.GetCurrent();
    var principal = new System.Security.Principal.WindowsPrincipal(identity);
    return principal.IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator);
}

public static void SetStartup(bool enableStartup)
{
    if (!IsAdministrator())
    {
        _logger.LogWarning("Operation may require administrator privileges");
    }

    // 既存のコード...
}
```

### 5. 非同期APIの提供

```csharp
// 現状
// 同期APIのみ

// 提案
// 非同期APIの追加
public static async Task SetStartupAsync(bool enableStartup)
{
    await Task.Run(() => SetStartup(enableStartup)).ConfigureAwait(false);
}

public static async Task<bool> IsRegisteredStartupAsync()
{
    return await Task.Run(() => IsRegisteredStartup()).ConfigureAwait(false);
}

public static async Task EnsureStartupRegistrationAsync()
{
    await Task.Run(() => EnsureStartupRegistration()).ConfigureAwait(false);
}
```

## セキュリティ上の懸念

1. **レジストリアクセス権限** - レジストリ操作は権限が必要であり、アクセス拒否の可能性があります。管理者権限の有無をチェックし、必要に応じてユーザーに通知すべきです。
2. **パスインジェクション** - レジストリに保存されるパスは適切にサニタイズされるべきです。現在は二重引用符で囲まれていますが、さらなるチェックが必要かもしれません。

## パフォーマンス上の懸念

特に重大なパフォーマンス上の問題は見当たりません。レジストリ操作は比較的軽量です。

## 命名規則

命名規則は適切に守られており、C#の標準的な命名規則（PascalCase、camelCase）に従っています。

## まとめ

全体的に機能的なレジストリ管理クラスですが、インターフェースの導入、例外処理の強化、構造化ログの導入、管理者権限の確認、および非同期APIの提供により、コードの保守性、テスト容易性、および堅牢性を向上させることができます。特に、例外処理の強化と管理者権限の確認は、アプリケーションの安定性を高めるために重要な改善点です。

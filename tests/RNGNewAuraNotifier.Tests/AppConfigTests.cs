using System;
using System.IO;
using Xunit;
using RNGNewAuraNotifier;

namespace RNGNewAuraNotifier.Tests
{
  public class AppConfigTests : IDisposable
  {
    private readonly string _originalDirectory;
    private readonly string _tempDirectory;

    public AppConfigTests()
    {
      _originalDirectory = Environment.CurrentDirectory;
      _tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
      Directory.CreateDirectory(_tempDirectory);
      Environment.CurrentDirectory = _tempDirectory;
    }

    public void Dispose()
    {
      Environment.CurrentDirectory = _originalDirectory;
      if (Directory.Exists(_tempDirectory))
      {
        Directory.Delete(_tempDirectory, true);
      }
    }

    [Fact]
    // 設定ファイルが存在しない場合、LogDirがnullを返すことを確認します。
    public void LogDir_Default_ReturnsNull()
    {
      Assert.Null(AppConfig.LogDir);
    }

    [Fact]
    // 有効なディレクトリをLogDirに設定すると、その値が保持され、設定ファイルが更新されることを確認します。
    public void LogDir_SetValidDirectory_PersistsValue()
    {
      var dir = Path.Combine(_tempDirectory, "logs");
      Directory.CreateDirectory(dir);
      AppConfig.LogDir = dir;
      Assert.Equal(dir, AppConfig.LogDir);

      var configFile = Path.Combine(_tempDirectory, "config.json");
      Assert.True(File.Exists(configFile));
      var json = File.ReadAllText(configFile);
      Assert.Contains("logDir", json);
      Assert.Contains(dir, json);
    }

    [Fact]
    // 無効なディレクトリをLogDirに設定すると例外がスローされることを確認します。
    public void LogDir_SetInvalidDirectory_Throws()
    {
      var dir = Path.Combine(_tempDirectory, "nonexistent");
      var ex = Assert.Throws<DirectoryNotFoundException>(() => AppConfig.LogDir = dir);
      Assert.Contains("does not exist", ex.Message);
    }

    [Fact]
    // LogDirをnullに設定すると値がクリアされることを確認します。
    public void LogDir_SetNull_ClearsValue()
    {
      var dir = Path.Combine(_tempDirectory, "logs2");
      Directory.CreateDirectory(dir);
      AppConfig.LogDir = dir;
      Assert.Equal(dir, AppConfig.LogDir);

      AppConfig.LogDir = null;
      Assert.Null(AppConfig.LogDir);
    }

    [Fact]
    // 設定ファイルが存在しない場合、DiscordWebhookUrlがnullを返すことを確認します。
    public void DiscordWebhookUrl_Default_ReturnsNull()
    {
      Assert.Null(AppConfig.DiscordWebhookUrl);
    }

    [Theory]
    [InlineData("http://example.com/hook")]
    [InlineData("https://localhost/webhook")]
    // 有効なURLをDiscordWebhookUrlに設定すると、その値が保持され、設定ファイルが更新されることを確認します。
    public void DiscordWebhookUrl_SetValidUrl_Persists(string url)
    {
      AppConfig.DiscordWebhookUrl = url;
      Assert.Equal(url, AppConfig.DiscordWebhookUrl);

      var json = File.ReadAllText(Path.Combine(_tempDirectory, "config.json"));
      Assert.Contains("discordWebhookUrl", json);
      Assert.Contains(url, json);
    }

    [Theory]
    [InlineData("ftp://example.com")]
    [InlineData("example.com")]
    // 無効なURLをDiscordWebhookUrlに設定すると例外がスローされることを確認します。
    public void DiscordWebhookUrl_SetInvalidUrl_Throws(string url)
    {
      var ex = Assert.Throws<ArgumentException>(() => AppConfig.DiscordWebhookUrl = url);
      Assert.Contains("must start with http", ex.Message);
    }

    [Fact]
    // DiscordWebhookUrlをnullに設定すると値がクリアされることを確認します。
    public void DiscordWebhookUrl_SetNull_ClearsValue()
    {
      AppConfig.DiscordWebhookUrl = "http://example.com";
      Assert.Equal("http://example.com", AppConfig.DiscordWebhookUrl);

      AppConfig.DiscordWebhookUrl = null;
      Assert.Null(AppConfig.DiscordWebhookUrl);
    }
  }
}
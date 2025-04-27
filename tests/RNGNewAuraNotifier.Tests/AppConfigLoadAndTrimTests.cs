using System;
using System.IO;
using System.Text.Json;
using Xunit;
using RNGNewAuraNotifier;

namespace RNGNewAuraNotifier.Tests
{
  public class AppConfigLoadAndTrimTests : IDisposable
  {
    private readonly string _tempDir;
    private readonly string _configPath;

    public AppConfigLoadAndTrimTests()
    {
      _tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
      Directory.CreateDirectory(_tempDir);
      Environment.CurrentDirectory = _tempDir;
      _configPath = Path.Combine(_tempDir, "config.json");
    }

    public void Dispose()
    {
      // cleanup
      try { Directory.Delete(_tempDir, true); } catch { }
    }

    [Fact]
    public void LogDir_TrimmedValue_IsTrimmed()
    {
      var dir = _tempDir;
      var input = "  " + dir + "  ";
      AppConfig.LogDir = input;
      Assert.Equal(dir, AppConfig.LogDir);
    }

    [Fact]
    public void DiscordWebhookUrl_TrimmedValue_IsTrimmed()
    {
      var url = "http://example.com/hook";
      var input = "  " + url + "  ";
      AppConfig.DiscordWebhookUrl = input;
      Assert.Equal(url, AppConfig.DiscordWebhookUrl);
    }

    [Fact]
    public void Load_ExistingConfig_IsLoadedCorrectly()
    {
      // create config.json manually
      var data = new { logDir = _tempDir, discordWebhookUrl = "http://abc" };
      var json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
      File.WriteAllText(_configPath, json);

      // reload static state
      // access private Load method
      var load = typeof(AppConfig).GetMethod("Load", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
      load.Invoke(null, null);

      Assert.Equal(_tempDir, AppConfig.LogDir);
      Assert.Equal("http://abc", AppConfig.DiscordWebhookUrl);
    }
  }
}
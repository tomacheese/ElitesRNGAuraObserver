using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using RNGNewAuraNotifier;

namespace RNGNewAuraNotifier.Tests
{
  public class VRChatLogWatcherTests : IDisposable
  {
    private readonly string _tempDir;
    private readonly VRChatLogWatcher _watcher;

    public VRChatLogWatcherTests()
    {
      _tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
      Directory.CreateDirectory(_tempDir);
      _watcher = new VRChatLogWatcher(_tempDir);
    }

    public void Dispose()
    {
      try { Directory.Delete(_tempDir, true); } catch { }
    }

    // デフォルトのディレクトリが正しく使用されることを確認します。
    [Fact]
    public void Constructor_NullDirectory_UsesDefaultPath()
    {
      var defaultWatcher = new VRChatLogWatcher(null);
      var defaultDir = typeof(VRChatLogWatcher)
          .GetField("logDir", BindingFlags.NonPublic | BindingFlags.Instance)
          .GetValue(defaultWatcher) as string;
      Assert.False(string.IsNullOrEmpty(defaultDir));
      // default path contains LocalLow\VRChat\VRChat
      Assert.Contains("LocalLow", defaultDir);
      Assert.Contains("VRChat", defaultDir);
    }

    [Fact]
    public void GetLogDir_ReturnsProvidedDirectory()
    {
      Assert.Equal(_tempDir, _watcher.GetLogDir());
    }

    [Fact]
    public void GetNewestLogFile_NoFiles_ReturnsNull()
    {
      var newest = _watcher.GetType()
          .GetMethod("GetNewestLogFile", BindingFlags.NonPublic | BindingFlags.Instance)
          .Invoke(_watcher, null);
      Assert.Null(newest);
    }

    [Fact]
    public void GetNewestLogFile_MultipleFiles_ReturnsLatest()
    {
      var files = new[] { "output_log_1.txt", "output_log_2.txt", "output_log_3.txt" };
      for (int i = 0; i < files.Length; i++)
      {
        var path = Path.Combine(_tempDir, files[i]);
        File.WriteAllText(path, "test");
        // update write time
        File.SetLastWriteTime(path, DateTime.Now.AddMinutes(i));
      }
      var newest = _watcher.GetType()
          .GetMethod("GetNewestLogFile", BindingFlags.NonPublic | BindingFlags.Instance)
          .Invoke(_watcher, null) as string;
      Assert.EndsWith("output_log_3.txt", newest);
    }

    [Fact]
    public async Task ReadNewLines_NonexistentFile_DoesNotThrowAsync()
    {
      var readNew = typeof(VRChatLogWatcher)
          .GetMethod("ReadNewLines", BindingFlags.NonPublic | BindingFlags.Instance);
      var ex = await Record.ExceptionAsync(() => (Task)readNew.Invoke(_watcher, new object[] { Path.Combine(_tempDir, "no.txt") }));
      Assert.Null(ex);
    }

    [Fact]
    public async Task ReadNewLines_FileSizeReduced_ResetsOffset()
    {
      var filePath = Path.Combine(_tempDir, "output_log_test.txt");
      File.WriteAllText(filePath, "line1\nline2");
      // set offset larger than file length
      var lastOffsetField = typeof(VRChatLogWatcher).GetField("lastOffset", BindingFlags.NonPublic | BindingFlags.Instance);
      lastOffsetField.SetValue(_watcher, 100L);

      var readNew = typeof(VRChatLogWatcher)
          .GetMethod("ReadNewLines", BindingFlags.NonPublic | BindingFlags.Instance);
      await (Task)readNew.Invoke(_watcher, new object[] { filePath });

      long newOffset = (long)lastOffsetField.GetValue(_watcher);
      var fileLength = new FileInfo(filePath).Length;
      Assert.Equal(fileLength, newOffset);
    }
  }
}
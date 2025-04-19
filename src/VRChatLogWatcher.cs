using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace RNGNewAuraNotifier
{
    /// <summary>
    /// VRChatのログファイルを監視するクラス
    /// summary>
    internal class VRChatLogWatcher
    {
        /// <summary>
        /// 監視対象のログディレクトリパス
        /// </summary>
        private readonly string logDir;

        /// <summary>
        /// 監視対象のログファイルパス
        /// </summary>
        private string currentFile = null;

        /// <summary>
        /// ログファイルの最後に読み込んだ位置
        /// </summary>
        private long lastOffset = 0;

        /// <summary>
        /// キャンセル用のトークンソース
        /// </summary>
        private readonly CancellationTokenSource cts = new CancellationTokenSource();

        /// <summary>
        /// 初回読み込み中かどうか
        /// </summary>
        private bool isFirstLoading = true;

        /// <summary>
        /// VRChatユーザ情報
        /// </summary>
        private VRChatUser vrchatUser = null;

        /// <summary>
        /// VRChatログイン時のログパターン
        /// </summary>
        private readonly string vrchatuserAuthenticatedLogPattern = @"(?<datetime>[0-9]{4}\.[0-9]{2}.[0-9]{2} [0-9]{2}:[0-9]{2}:[0-9]{2}) (?<Level>.[A-z]+) *- *User Authenticated: (?<UserName>.+) \((?<UserId>usr_[A-z0-9\-]+)\)";

        /// <summary>
        /// Aura取得時のログパターン
        /// </summary>
        private readonly string auraLogPattern = @"(?<datetime>[0-9]{4}\.[0-9]{2}.[0-9]{2} [0-9]{2}:[0-9]{2}:[0-9]{2}) (?<Level>.[A-z]+) *- *\[<color=green>Elite's RNG Land</color>\] Successfully legitimized Aura #(?<AuraId>[0-9]{2})\.";

        public VRChatLogWatcher(string logDirectory)
        {
            // ログディレクトリが指定されていない場合は、デフォルトのVRChatログディレクトリを使用する
            string defaultLogDir = Path.GetFullPath(Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                @"..\LocalLow\VRChat\VRChat"
            ));
            logDir = logDirectory ?? defaultLogDir;
        }

        public void Start()
        {
            Task.Run(async () =>
            {
                // 最初に一度読んで、その際は通知をしない
                await LoadLog();
                isFirstLoading = false;
                await MonitorLoop(cts.Token);
            });
        }

        public void Stop()
        {
            cts.Cancel();
        }

        public string GetLogDir()
        {
            return logDir;
        }

        public string GetCurrentFile()
        {
            return currentFile;
        }

        private async Task MonitorLoop(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                await LoadLog();
                await Task.Delay(1000);
            }
        }

        private async Task LoadLog()
        {
            string newestFile = GetNewestLogFile();
            if (!string.IsNullOrEmpty(newestFile))
            {
                if (newestFile != currentFile)
                {
                    Console.WriteLine($"Watching log file: {Path.GetFileName(newestFile)}");
                    currentFile = newestFile;
                    lastOffset = 0;
                }

                await ReadNewLines(currentFile);
            }
        }

        private string GetNewestLogFile()
        {
            var files = Directory.GetFiles(logDir, "output_log_*.txt");
            if(files.Length == 0)
            {
                return null;
            }
            return files.OrderByDescending(f => File.GetLastWriteTime(f)).FirstOrDefault();
        }

        private async Task ReadNewLines(string filePath)
        {
            if (filePath == null) return;
            if (!File.Exists(filePath)) return;

            var fi = new FileInfo(filePath);
            if (fi.Length < lastOffset)
            {
                lastOffset = 0;
            }

            using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                fs.Seek(lastOffset, SeekOrigin.Begin);
                using (var sr = new StreamReader(fs, Encoding.UTF8))
                {
                    string line;
                    while ((line = await sr.ReadLineAsync()) != null)
                    {
                        HandleLine(line);
                    }
                    lastOffset = fs.Position;
                }
            }
        }

        private void HandleLine(string line)
        {
            Console.WriteLine($"[LOG|{isFirstLoading}] {line}");

            // 2025.04.19 14:10:45 Debug      -  User Authenticated: Tomachi (usr_0b83d9be-9852-42dd-98e2-625062400acc)
            var matchUserLogPattern = Regex.Match(line, vrchatuserAuthenticatedLogPattern);
            if (matchUserLogPattern.Success)
            {
                string datetime = matchUserLogPattern.Groups["datetime"].Value;
                string level = matchUserLogPattern.Groups["Level"].Value;
                string userName = matchUserLogPattern.Groups["UserName"].Value;
                string userId = matchUserLogPattern.Groups["UserId"].Value;
                string message = $"[{datetime}] {level} - User Authenticated: {userName} ({userId})";
                Console.WriteLine($"[USER] {message}");
                vrchatUser = new VRChatUser
                {
                    UserName = userName,
                    UserId = userId
                };
            }

            // 2025.04.16 18:07:07 Debug      -  [<color=green>Elite's RNG Land</color>] Successfully legitimized Aura #60.
            var matchAuraLogPattern = Regex.Match(line, auraLogPattern);
            if (matchAuraLogPattern.Success)
            {
                string datetime = matchAuraLogPattern.Groups["datetime"].Value;
                string level = matchAuraLogPattern.Groups["Level"].Value;
                string auraId = matchAuraLogPattern.Groups["AuraId"].Value;
                string message = $"[{datetime}] {level} - [<color=green>Elite's RNG Land</color>] Successfully legitimized Aura #{auraId}.";
                Console.WriteLine($"[NOTIFY] {message}");

                string auraName = Aura.GetAuraName(auraId);

                if (!isFirstLoading)
                {
                    Notifier.ShowToast("Successfully legitimized Aura!", $"{auraName} (#{auraId})");
                    Task.Run(async () =>
                    {
                        await Notifier.SendDiscordWebhook("Successfully legitimized Aura!", $"{auraName} (#{auraId})", vrchatUser);
                    });
                }
            }

        }
    }
}

# Copilot コードレビュー指示

プロジェクトの概要、構成、コマンド、コーディング規約は [AGENTS.md](../AGENTS.md) にある。このファイルにはレビューで重視する点だけを書く。レビューコメントは日本語で書く。

コメントは具体的で、対応できる内容にする。

## レビューの重点

- **秘密情報の扱い。** Discord Webhook URL (`ConfigData.DiscordWebhookUrl`) はユーザーが入力する機密情報である。ログやエラー出力、テレメトリに出していないか、本物の Webhook やトークンをリテラルとして埋め込んでいないかを指摘する。
- **XML ドキュメント。** StyleCop (`stylecop.json`) は `private` を含む *すべて* の型とメンバーに doc コメントを要求し、無いと CI の `dotnet format --verify-no-changes` が失敗する。新規・変更したメンバーに doc コメントが無い場合と、シグネチャと合わなくなった `<param>` / `<returns>` タグを指摘する。
- **ログ解析の正規表現。** 検知は VRChat のログ形式そのものに依存する (`NewAuraDetectionService.cs`、`AuthenticatedDetectionService.cs`)。意図より狭い・広い正規表現の変更や、実際のログ行に黙ってマッチしなくなる変更を指摘する。
- **監視ループの耐障害性。** `LogWatcher` と通知のコードはバックグラウンドのループで動く。ループを落とし得る例外の漏れを指摘する。ネットワークや通知の呼び出しは try/catch で囲み、ログに残すこと。
- **Nullable。** `Nullable` は有効である。null になり得る値の参照と、根拠のない `!` (null 許容抑制演算子) を指摘する。
- **ファイル / IO とプロセス操作。** アップデーターはディレクトリの削除、プロセスの終了、アセットのダウンロード、展開前の sha256 ダイジェスト検証を行う。ダイジェスト検証の削除、展開先から外へ出得るパス処理、未処理の IO 失敗を指摘する。
- **リソースの寿命。** `IDisposable` な型 (`LogWatcher`、mutex、ストリーム、`CancellationTokenSource`) は破棄すること。リークや二重破棄を指摘する。
- **言語。** 新規・編集したコードコメント、ドキュメント、コミットや PR の文章は日本語で書く (AGENTS.md を参照)。英語のユーザー向けドキュメント (`README.md`、`docs/en/**`) と UI 文字列、英語のエラーメッセージは例外とする。

## 既知の問題でないもの (指摘しない)

- `README.md` と `docs/en/**` は意図的に英語である。日本語版は `README-ja.md` と `docs/ja/**` にある。
- Windows 専用の API (WinForms、レジストリ、`LibraryImport` / P/Invoke、UWP トースト) は想定どおりである。クロスプラットフォームの代替を提案しない。
- `AllowUnsafeBlocks` と全体の `Console.WriteLine` によるデバッグログは意図的である。
- アプリとアップデーターで `SemanticVersion` / `ReleaseInfo` / `GitHubReleaseService` が重複しているのは意図的である (アップデーターは単独で動く)。
- 単体テストが無いことは既知である。テストプロジェクトの追加を必須の指摘にしない。
- `Auras.json` は生成された大きなデータカタログである。1 行ずつレビューしない。

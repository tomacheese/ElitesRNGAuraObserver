# AGENTS.md

## 言語

- 成果物は **日本語** で書く。対象はコミットメッセージの説明部分 (Conventional Commits の type は英語)、PR / Issue のタイトルと本文、レビューコメント、コードコメントと XML ドキュメントコメント、ドキュメントである。
- 例外は次のとおり。ローカライズされた UI 文字列 (UI は英語)、引用、技術的な識別子、生成ファイル、外部ソースのテキスト、英語のユーザー向けドキュメント (`README.md`、`docs/en/**`)、英語で書くエラーメッセージ。日本語版の `README-ja.md` と `docs/ja/**` は日本語のまま維持し、英語版と内容を揃える。
- 既存コードのコメントは日本語であり、新規・編集するコメントも日本語のまま書く。
- 日本語と半角英数字の間には半角スペースを入れる。散文の段落の途中で手動改行しない。

## 概要

Elite's RNG Aura Observer は Windows 向けのデスクトップアプリ (C# / WinForms、.NET 9) である。VRChat のワールド "Elite's RNG Land" でプレイヤーが Aura を入手すると、VRChat のログファイルからそれを検知し、Windows のトースト通知と Discord Webhook メッセージを送る。タスクトレイに常駐してバックグラウンドで動作し、別プロセスの自動アップデーターを同梱する。Windows 専用。

## ソリューション構成

`ElitesRNGAuraObserver.sln` には 2 つのプロジェクトがある。

- `ElitesRNGAuraObserver/` — メインのトレイアプリ (`WinExe`)。
  - `Program.cs` — エントリポイント。多重起動防止の mutex、例外ハンドラー、アップデート確認を行い、`Application.Run(new TrayIcon())` を呼ぶ。
  - `Core/AuraObserverController.cs` — `LogWatcher` と各検知サービスをつなぐ。
  - `Core/VRChat/LogWatcher.cs` — 最新の `output_log_*.txt` を 1 秒ごとにポーリングし、`OnNewLogLine` を発火する。
  - `Core/VRChat/AuthenticatedDetectionService.cs`、`Core/Aura/NewAuraDetectionService.cs` — ログ行を正規表現で解析してイベントにする。
  - `Core/Notification/UwpNotificationService.cs`、`DiscordNotificationService.cs` — トースト通知と Webhook の送信。
  - `Core/Config/` — `AppConfig` (config.json の読み書き)、`ConfigData`、`RegistryManager` (スタートアップ登録)。
  - `Core/Aura/` — `Aura` レコードと `AuraCategory` 列挙型。`Core/Json/JsonData.cs` はカタログのローダー。Aura のカタログは `Resources/Auras.json` (埋め込み) で、ローカルの `Auras.json` で上書きできる。
  - `Core/Updater/` — GitHub Releases を確認し、`ElitesRNGAuraObserver.Updater.exe` を起動する。`--skip-update` で確認をスキップでき、`--debug` でコンソールを開く。
  - `UI/` — `SettingsForm`、`TrayIcon`、カスタムコントロール。
- `ElitesRNGAuraObserver.Updater/` — 単独で動く self-contained (`win-x64`) のコンソールアップデーター。最新の GitHub Release のアセットをダウンロードして sha256 ダイジェストを検証し、実行中のアプリを終了 (正常終了を試み、だめなら強制終了) してから展開・再起動する。メインアプリとコードを共有しない設計なので、`SemanticVersion`、`ReleaseInfo`、`GitHubReleaseService` は両プロジェクトに存在する。コピー同士の内容を揃えること。

## 開発コマンド

- `dotnet restore ElitesRNGAuraObserver.sln` — パッケージの復元。
- `dotnet build ElitesRNGAuraObserver.sln /p:Configuration=Release` — ビルド。
- `dotnet publish ElitesRNGAuraObserver.sln -p:PublishProfile=Publish` — 単一ファイルを `bin/Publish/` に発行する。
- `dotnet format ElitesRNGAuraObserver.sln --verify-no-changes --severity warn` — スタイルチェック。CI がこのコマンドをそのまま実行し、差分があれば失敗する。コミット前に実行すること。

CI は Windows (`net9.0-windows10.0.17763.0`、WinForms) でビルドする。Linux では `EnableWindowsTargeting=true` (環境変数または `-p:`) を付ければ `build` できる。ただし `.editorconfig` が CRLF を要求するため、LF でチェックアウトすると `dotnet format` が行末エラーを報告する。Linux では代わりに `dotnet format analyzers --verify-no-changes --severity warn --no-restore` を使う。

## テスト

自動テストのプロジェクトはない。変更後は手動で確認する。ビルドしてアプリを起動し、実際または sample の VRChat ログで検知と通知を確かめる。`NewAuraDetectionService.cs` の Aura 検知の正規表現は VRChat のログ形式そのものに依存する。変更するときは実際の `output_log_*.txt` の行でテストすること。

## コーディング規約

- StyleCop.Analyzers を `stylecop.json` で有効にしている。`documentPrivateElements` / `documentPrivateFields` が true なので、`private` を含む **すべて** の型とメンバーに XML ドキュメントコメントが必要で、無いとスタイルチェックが失敗する。
- インデントは 4 スペース (タブ不可)。ファイルスコープの名前空間を使う。`using` は名前空間の外に置き、`System.*` を先頭にする。ファイル末尾に改行が必要。
- `Nullable` と `ImplicitUsings` は有効。null 許容の注釈を守る。
- `<param>` / `<returns>` タグはシグネチャと揃える (StyleCop のドキュメント規則はコメントの欠落を検査するだけで、古いタグは検出しない)。
- 通知やネットワークの呼び出しは try/catch で囲み、失敗は `Console.WriteLine` でログに残して、監視ループを落とさないようにする。`AuraObserverController` の Discord 呼び出しがその例。トースト通知の呼び出しは現状 try/catch で囲まれていない。

## 設定と実行時データ

- ユーザー設定は `%LocalAppData%\tomacheese\ElitesRNGAuraObserver` 配下の `config.json`。exe の隣に置いた任意の `config.path` ファイルで設定ディレクトリを変更できる。
- `ConfigData` には Discord Webhook URL、トースト通知の ON/OFF、VRChat のログディレクトリ、Auras.json のディレクトリを持つ。Webhook URL はユーザーの秘密情報なので、ログに出したり本物の値をコミットしたりしないこと。
- `RegistryManager.EnsureStartupRegistration()` は、スタートアップ起動用の Windows レジストリキーを書き込む。

## CI / リリース

- `.github/workflows/ci.yml` — `main` / `master` への push と PR で、復元、ビルド、発行、成果物のアップロード、`dotnet format` によるスタイルチェックを行う。
- `.github/workflows/release.yml` — `main` / `master` への push で、`mathieudutour/github-tag-action` が Conventional Commits のプレフィックスからバージョンを上げ、GitHub Release の zip をビルド・発行する。
- コミットメッセージは Conventional Commits (`feat:`、`fix:`、`chore:` など) に従う。プレフィックスがリリースのバージョン更新を決めるので、正確に付けること。

## ドキュメント

- ユーザー向けドキュメントは `docs/en/` と `docs/ja/` にあり、入口は `README.md` / `README-ja.md`。ユーザー向けの挙動 (インストール手順、設定、通知) を変えたら、両言語版を更新する。ドキュメントで引用する UI ラベルは `SettingsForm.Designer.cs` の文字列と一致させること。

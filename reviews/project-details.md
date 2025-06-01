# プロジェクト詳細

## プロジェクト概要

「Elite's RNG Aura Observer」は、VRChatのワールド「Elite's RNG Land」でオーラ（Aura）を獲得したときに、Windowsトースト通知とDiscord Webhookを通じて通知を送信するアプリケーションです。ユーザーがAFK中や離席中でも、重要なAuraの獲得を即座に知ることができるように設計されています。

### 主な機能

- VRChatのログファイルを監視し、オーラ獲得時のログを検出して通知
- AFK時や離席中でもオーラ獲得を即時に把握可能
- Elite's RNG Landでの演出があるオーラのみに通知対象を限定
- タスクトレイでバックグラウンド実行され、邪魔にならない設計
- 自動アップデート機能を搭載し、ユーザー操作なしでアップデート可能
- 設定ウィンドウで各種設定を容易に行える

## 技術スタック

### 言語・フレームワーク

- **プログラミング言語**: C#
- **フレームワーク**: .NET 9.0 (Windows 10.0.17763.0以上をターゲット)
- **UI**: Windows Forms

### 依存パッケージ・ライブラリ

#### メインアプリケーション (ElitesRNGAuraObserver)

- **Discord.Net.Webhook (3.17.4)**: Discord WebhookによるDiscordへの通知機能
- **Microsoft.Toolkit.Uwp.Notifications (7.1.3)**: Windowsトースト通知機能
- **Newtonsoft.Json (13.0.3)**: JSON処理
- **StyleCop.Analyzers (1.1.118)**: コードスタイル分析ツール

#### アップデーターアプリケーション (ElitesRNGAuraObserver.Updater)

- **Newtonsoft.Json (13.0.3)**: JSON処理
- **StyleCop.Analyzers (1.1.118)**: コードスタイル分析ツール

### ビルド設定

- シングルファイル公開 (PublishSingleFile)
- 自己完結型アプリケーション (SelfContained)
- Win-x64ランタイム対象
- 埋め込みデバッグ情報

## プロジェクト構成

プロジェクトは2つのアプリケーションから構成されています：

1. **ElitesRNGAuraObserver**: メインアプリケーション
   - Windows Forms ベースのGUIアプリケーション
   - タスクトレイに常駐し、VRChatのログを監視
   - オーラ獲得時の通知機能を提供

2. **ElitesRNGAuraObserver.Updater**: アップデーターアプリケーション
   - コンソールアプリケーション
   - メインアプリケーションの自動更新を担当

## ディレクトリ構成

### メインアプリケーション (ElitesRNGAuraObserver)

- **Core/**: コア機能を含むディレクトリ
  - **Aura/**: オーラ関連の機能
  - **Config/**: アプリケーション設定
  - **Json/**: JSON処理
  - **Notification/**: 通知機能
  - **Updater/**: アップデート機能
  - **VRChat/**: VRChatログ監視機能
- **UI/**: ユーザーインターフェース
  - **Controls/**: カスタムコントロール
  - **Settings/**: 設定画面
  - **TrayIcon/**: タスクトレイアイコン
- **Resources/**: リソースファイル
- **Properties/**: アプリケーションプロパティ

### アップデーターアプリケーション (ElitesRNGAuraObserver.Updater)

- **Core/**: コア機能
  - アップデート処理に必要な機能
- **Properties/**: アプリケーションプロパティ

## テストコード

現在のプロジェクト構成では、専用のテストプロジェクトやテストコードは確認できません。コードの品質は主にStyleCop.Analyzersを通じた静的解析によって維持されています。

## パッケージマネージャ

- **NuGet**: .NETのパッケージ管理に利用

## 配布形式

- シングルファイル実行ファイル (.exe) によるポータブルインストール
- 自己完結型でランタイムを同梱

## ライセンス

MIT License のもとで配布されています。ソースコードはGitHubで公開されており、寄付も受け付けています。

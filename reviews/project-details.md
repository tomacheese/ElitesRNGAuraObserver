# プロジェクト詳細

## プロジェクト概要

Elite's RNG Aura Observer は、VRChatの世界「Elite's RNG Land」でAuraが獲得された際に、WindowsのToast通知とDiscord Webhookによる通知を送信するC#製のデスクトップアプリケーションです。

## 使用技術スタック

### 開発言語・フレームワーク

- **言語**: C# 12.0（.NET 9.0）
- **フレームワーク**: Windows Forms
- **ターゲットプラットフォーム**: Windows 10.0.17763.0以降

### 主要な依存パッケージ・ライブラリ

#### メインアプリケーション（ElitesRNGAuraObserver）

- **Discord.Net.Webhook** (v3.17.4): Discord Webhook機能の実装
- **Microsoft.Toolkit.Uwp.Notifications** (v7.1.3): Windows Toast通知機能
- **Newtonsoft.Json** (v13.0.3): JSON処理
- **StyleCop.Analyzers** (v1.1.118): コーディング規約チェック

#### アップデーターアプリケーション（ElitesRNGAuraObserver.Updater）

- **Newtonsoft.Json** (v13.0.3): JSON処理
- **StyleCop.Analyzers** (v1.1.118): コーディング規約チェック

### パッケージマネージャ

- **NuGet**: .NET標準パッケージマネージャ
- **Renovate**: 依存関係の自動更新

## プロジェクト構成

### ソリューション構造

```
ElitesRNGAuraObserver.sln
├── ElitesRNGAuraObserver/           # メインアプリケーション
│   ├── Core/                        # コア機能
│   │   ├── Aura/                    # Aura検出機能
│   │   ├── Config/                  # 設定管理
│   │   ├── Json/                    # JSON処理
│   │   ├── Notification/            # 通知機能
│   │   ├── Updater/                 # 更新機能
│   │   └── VRChat/                  # VRChat関連機能
│   ├── UI/                          # ユーザーインターフェース
│   │   ├── Controls/                # カスタムコントロール
│   │   ├── Settings/                # 設定画面
│   │   └── TrayIcon/                # システムトレイ
│   └── Resources/                   # リソースファイル
└── ElitesRNGAuraObserver.Updater/   # 自動更新ツール
    └── Core/                        # アップデーター機能
```

### ディレクトリ詳細

- **Core/**: アプリケーションの中核機能を実装
- **UI/**: ユーザーインターフェース関連のコンポーネント
- **Resources/**: アプリケーションアイコンやAura定義JSON
- **docs/**: プロジェクトドキュメント（英語・日本語対応）

## CI/CD パイプライン

### GitHub Actions ワークフロー

1. **Build (ci.yml)**:
   - .NET 9.0環境でのビルド
   - NuGetパッケージキャッシュ（`~/.nuget/packages`）
   - `dotnet restore`、`dotnet build`、`dotnet publish`の実行
   - ビルドアーティファクトのアップロード
   - `dotnet format --verify-no-changes --severity warn` によるコードスタイルチェック

2. **Release (release.yml)**:
   - セマンティックバージョニング（`mathieudutour/github-tag-action`）
   - 自動バージョンバンプ（デフォルト：minor）
   - カスタムリリースルール（`feat:minor`、`fix:patch`など）
   - PowerShellによる.csprojファイルのバージョン自動更新
   - Zip形式でのリリースアーティファクトの作成
   - GitHubリリースの自動作成（`softprops/action-gh-release`）

3. **Assign Review (review.yml)**:
   - プルリクエスト作成時の自動レビュー担当者割り当て
   - `kentaro-m/auto-assign-action` を使用

### コード品質管理

- **StyleCop.Analyzers**: C#コーディング規約の強制
- **dotnet format**: コードフォーマッタによる統一
- **.editorconfig**: エディタ設定とコードスタイルの詳細な統一
  - 4スペースインデント、CRLF改行コード、UTF-8エンコーディング
  - 詳細なC#コードスタイル設定（ブレース配置、式本体メソッド、命名規則など）
  - 170以上のIDE診断ルールの警告レベル設定
  - StyleCop.Analyzersの詳細な設定（50以上のルール）
- **Renovate**: 依存関係の自動更新（`book000/templates//renovate/base-public`ベース）

## テストコード

現在のプロジェクト構造を確認した結果、専用のテストプロジェクトは含まれていません。テストはCIパイプラインでのビルド検証のみが実行されています。

## ビルド・デプロイメント

### ビルド設定

- **PublishSingleFile**: 単一実行ファイルとして出力
- **SelfContained**: 自己完結型デプロイメント（Updaterのみ）
- **RuntimeIdentifier**: win-x64プラットフォーム指定

### リリース形式

- ZIP形式での配布
- ポータブル実行ファイル（インストール不要）
- 自動更新機能内蔵

## 外部連携

- **VRChat**: ログファイル監視による連携
- **Discord**: Webhook API経由での通知送信
- **GitHub**: リリース情報の自動取得
- **Elite's RNG Land**: VRChat世界のAuraデータ
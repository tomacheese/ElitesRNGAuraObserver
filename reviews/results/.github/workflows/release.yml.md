# GitHub Actions ワークフローレビュー: release.yml

## 概要
自動リリースパイプラインを実装するワークフローファイル。バージョンバンプ、ビルド、GitHub Releaseの作成を自動化。

## レビュー結果

### ✅ 良い点

1. **適切なワークフロー設計**
   - 2段階のジョブ構成（バージョンバンプ → ビルド＆リリース）
   - ジョブ間の依存関係と出力の適切な管理
   - concurrency設定でリリースプロセスの重複実行を防止

2. **自動バージョン管理**
   - mathieudutour/github-tag-actionによる自動バージョニング
   - コミットメッセージベースの詳細なリリースルール設定
   - 包括的なchangelog生成

3. **最新のアクション使用**
   - actions/checkout@v4, actions/setup-dotnet@v4, actions/cache@v4
   - softprops/action-gh-release@v2で安定したリリース作成

4. **効率的なキャッシュ戦略**
   - CIワークフローと同一のNuGetキャッシュ実装

### ⚠️ 改善点

1. **セキュリティ設定**
   - **重要度: 高** - permissionsの明示的設定が不足
   - GITHUB_TOKENの権限が過度に広範囲の可能性

2. **エラーハンドリング**
   - **重要度: 高** - バージョン設定スクリプトのエラーハンドリング不足
   - PowerShellスクリプトの実行失敗時の処理が不十分

3. **ビルド検証**
   - **重要度: 中** - リリース前のテスト実行が不足
   - コード品質チェックがスキップされている

4. **アーティファクト管理**
   - **重要度: 中** - ZIP作成プロセスの検証不足
   - 複数プラットフォーム対応の欠如

5. **リリース設定**
   - **重要度: 低** - プレリリース機能の未活用
   - リリースノートのカスタマイズ不足

### 🔧 推奨改善案

```yaml
name: Release

on:
  push:
    branches:
      - main
      - master
  workflow_dispatch:

permissions:
  contents: write
  packages: write
  actions: read

concurrency:
  group: ${{ github.workflow }}-${{ github.ref }}
  cancel-in-progress: false

jobs:
  bump-version:
    name: Bump version
    runs-on: ubuntu-latest

    outputs:
      version: ${{ steps.tag-version.outputs.new_version }}
      tag: ${{ steps.tag-version.outputs.new_tag }}
      changelog: ${{ steps.tag-version.outputs.changelog }}

    steps:
      - name: Checkout
        uses: actions/checkout@v4
        with:
          fetch-depth: 0

      - name: Bump version and push tag
        id: tag-version
        uses: mathieudutour/github-tag-action@v6.2
        with:
          github_token: ${{ secrets.GITHUB_TOKEN }}
          default_bump: "minor"
          custom_release_rules: "feat:minor:✨ Features,fix:patch:🐛 Fixes,docs:patch:📰 Docs,chore:patch:🎨 Chore,pref:patch:🎈 Performance improvements,refactor:patch:🧹 Refactoring,build:patch:🔍 Build,ci:patch:🔍 CI,revert:patch:⏪ Revert,style:patch:🧹 Style,test:patch:👀 Test,release:major:📦 Release"

  build-and-release:
    name: Build and Release
    runs-on: windows-latest
    needs: bump-version
    if: needs.bump-version.outputs.version != ''

    env:
      version: ${{ needs.bump-version.outputs.version }}

    steps:
      - name: Checkout repository
        uses: actions/checkout@v4

      - name: Setup .NET
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: "9.0.x"

      - name: Cache NuGet packages
        uses: actions/cache@v4
        with:
          path: ~/.nuget/packages
          key: ${{ runner.os }}-nuget-${{ hashFiles('**/*.csproj') }}
          restore-keys: |
            ${{ runner.os }}-nuget-

      - name: Set version from tag
        run: |
          try {
            $version = $env:APP_VERSION
            if (-not $version) {
              throw "Version not found"
            }
            
            $versionWithRevision = ($version.Split('.') + '0')[0..3] -join '.'
            
            # Update main project
            $mainProject = ".\ElitesRNGAuraObserver\ElitesRNGAuraObserver.csproj"
            if (Test-Path $mainProject) {
              (Get-Content $mainProject) -replace '<Version>[^<]*(</Version>)', "<Version>$version</Version>" | Set-Content $mainProject
              (Get-Content $mainProject) -replace '<AssemblyVersion>[^<]*</AssemblyVersion>', "<AssemblyVersion>$versionWithRevision</AssemblyVersion>" | Set-Content $mainProject
              (Get-Content $mainProject) -replace '<FileVersion>[^<]*</FileVersion>', "<FileVersion>$versionWithRevision</FileVersion>" | Set-Content $mainProject
            }
            
            # Update updater project
            $updaterProject = ".\ElitesRNGAuraObserver.Updater\ElitesRNGAuraObserver.Updater.csproj"
            if (Test-Path $updaterProject) {
              (Get-Content $updaterProject) -replace '<Version>[^<]*(</Version>)', "<Version>$version</Version>" | Set-Content $updaterProject
              (Get-Content $updaterProject) -replace '<AssemblyVersion>[^<]*</AssemblyVersion>', "<AssemblyVersion>$versionWithRevision</AssemblyVersion>" | Set-Content $updaterProject
              (Get-Content $updaterProject) -replace '<FileVersion>[^<]*</FileVersion>', "<FileVersion>$versionWithRevision</FileVersion>" | Set-Content $updaterProject
            }
            
            Write-Host "Version updated to $version successfully"
          }
          catch {
            Write-Error "Failed to update version: $_"
            exit 1
          }
        shell: pwsh
        env:
          APP_VERSION: ${{ needs.bump-version.outputs.version }}

      - name: Restore dotnet packages
        run: dotnet restore ElitesRNGAuraObserver.sln

      - name: Run tests
        run: dotnet test ElitesRNGAuraObserver.sln --configuration Release --verbosity normal

      - name: Build and publish solution
        run: dotnet publish ElitesRNGAuraObserver.sln -p:PublishProfile=Publish --configuration Release

      - name: Create release package
        run: |
          try {
            $sourceDir = "bin/Publish"
            if (-not (Test-Path $sourceDir)) {
              throw "Publish directory not found: $sourceDir"
            }
            
            $zipFilePath = "ElitesRNGAuraObserver-${{ needs.bump-version.outputs.version }}.zip"
            $sourceFiles = Get-ChildItem -Path "$sourceDir\*" -Recurse
            
            if ($sourceFiles.Count -eq 0) {
              throw "No files found in publish directory"
            }
            
            Compress-Archive -Path "$sourceDir\*" -DestinationPath $zipFilePath -Force
            
            if (-not (Test-Path $zipFilePath)) {
              throw "Failed to create ZIP file"
            }
            
            $zipSize = (Get-Item $zipFilePath).Length
            Write-Host "Created release package: $zipFilePath ($zipSize bytes)"
          }
          catch {
            Write-Error "Failed to create release package: $_"
            exit 1
          }
        shell: pwsh

      - name: Publish Release
        uses: softprops/action-gh-release@v2
        with:
          body: ${{ needs.bump-version.outputs.changelog }}
          tag_name: ${{ needs.bump-version.outputs.tag }}
          target_commitish: ${{ github.sha }}
          files: ElitesRNGAuraObserver-${{ needs.bump-version.outputs.version }}.zip
          draft: false
          prerelease: ${{ contains(needs.bump-version.outputs.version, 'alpha') || contains(needs.bump-version.outputs.version, 'beta') || contains(needs.bump-version.outputs.version, 'rc') }}
```

### 📊 総合評価

- **セキュリティ**: ⚠️ 改善必要（権限設定の明示化）
- **信頼性**: ⚠️ 改善必要（エラーハンドリング強化）
- **自動化**: ✅ 優秀（包括的な自動リリース）
- **保守性**: ✅ 良好（明確なワークフロー構成）

### 🎯 重要度別改善タスク

**High Priority:**
1. permissions設定の追加
2. PowerShellスクリプトのエラーハンドリング強化
3. リリース前テスト実行の追加

**Medium Priority:**
1. ZIP作成プロセスの検証強化
2. プレリリース機能の実装
3. マルチプラットフォーム対応検討

**Low Priority:**
1. リリースノートのカスタマイズ
2. 成果物の署名実装

### 🚨 セキュリティ考慮事項

1. **最小権限の原則**: contents:write, packages:writeのみに制限
2. **トークン管理**: GITHUB_TOKENの適切な使用
3. **コード検証**: リリース前の包括的なテスト実行

## 関連ファイル
- ElitesRNGAuraObserver.sln
- ElitesRNGAuraObserver/ElitesRNGAuraObserver.csproj
- ElitesRNGAuraObserver.Updater/ElitesRNGAuraObserver.Updater.csproj
- bin/Publish/ (成果物出力先)
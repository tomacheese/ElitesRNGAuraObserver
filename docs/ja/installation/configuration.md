# アプリケーションの設定

このページでは、Elite's RNG Aura Observer で設定可能な項目、設定画面で確認できる事項について説明しています。

TODO: 設定画面の画像

設定画面を開くには、タスクバーでアプリケーションアイコンをダブルクリックするか、右クリックして `Settings` をクリックしてください。

## 監視しているログファイルパス

`Monitoring Information` の `Monitoring Log File` にて、Aura を獲得したかどうかを検出するために使用しているログファイルのパスを確認できます。

## Discord Webhook の URL

`Discord Settings` の `Discord Webhook URL` に Discord Webhook の URL を入力することで、通知先の設定が行えます。

Discord Webhook URL の取得方法については、以下のページをご覧ください。

- [Discord Webhook URL の取得方法](get-discord-webhook-url.md)

## Windows トースト通知での通知を行うかどうか

`Application Settings` の `Enable Toast notification` チェックボックスにチェックをいれることで、Aura 獲得時に Windows トースト通知での通知を行うようになります。

Windows トースト通知は、以下のように通知されます。

TODO: 画像

## Windows 起動時に起動するかどうか

`Application Settings` の `Start when Windows starts` チェックボックスにチェックをいれることで、Windows 起動時に Elite's RNG Aura Observer が起動するようになります。

## 設定ファイルの保存先変更

`Application Settings` の `Config File Directory` にフォルダパスを入力することで、設定ファイルの保存先を変更できます。
何も入力しない場合、`%USERPROFILE%\AppData\Local\tomacheese\ElitesRNGAuraObserver\` に格納されます。

設定ファイルのファイル名は `config.json`、Aura の情報を管理しているファイルのファイル名は `Auras.json` で、これは設定で変更することはできません。

## その他の情報

`About` にて、以下の情報を確認できます。

- `Application version`: Elite's RNG Aura Observer のバージョン
- `Aura data version`: Elite's RNG Land の Aura 情報のバージョン

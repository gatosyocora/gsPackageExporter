# gsPackageExporter

UnityPackageを出力するEditor拡張です
最新版 [v0.1](https://github.com/gatosyocora/gsPackageExporter/releases)

## 使い方
1. gsPackageExporterを使いたいProjectにインポートします
2. Assetsフォルダ内のアセットでUnityPackageとして出力したいものを選択します（複数選択可）
3. 選択したものを右クリックして"Export Package by gs"を選択します
4. 表示されたアセットを確認して"Export"を選択します
5. UnityPackageの保存先を選択して"OK"を選択します

## 機能
* 選択したものをUnityPackageとして出力（複数選択可）
* 選択したものを一覧で表示
* フォルダ選択時にそれ以下のアセットを出力する

* UnityPackageに含めたアセット以外を除外する.gitignoreの作成[未完成]
* 選択したものから出力時に任意のものを出力するものから除外する[未実装]
* 特定のフォルダ以下のものは常に除外するようにする[未実装]

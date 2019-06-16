# gsPackageExporter

UnityPackageを出力するEditor拡張です

最新版 [v0.2](https://github.com/gatosyocora/gsPackageExporter/releases)

![sample](https://github.com/gatosyocora/gsPackageExporter/blob/img/img/gsPackageExporter_sampleimg.png)

## 使い方
1. gsPackageExporterを使いたいProjectにインポートします
2. Assetsフォルダ内のアセットでUnityPackageとして出力したいものを選択します（複数選択可）
3. 選択したものを右クリックして"Export Package by gs"を選択します
4. 表示されたアセットを確認して"Export"を選択します
5. UnityPackageの保存先を選択して"OK"を選択します

生成される.gitignoreをUnityProjectを管理するフォルダ(Assetsフォルダなどと同じ場所)に配備すると
UnityPackageに含めたアセット以外はgit管理外にするようにします。

## 機能
* 選択したものをUnityPackageとして出力（複数選択可）
* 選択したものを一覧で表示
* フォルダ選択時にそれ以下のアセットを出力する
* UnityPackageに含めたアセット以外を除外する.gitignoreの作成

* [未実装]出力するアセットに使われているアセットも含めるようにする
* [未実装]選択したものから出力時に任意のものを出力するものから除外する
* [未実装]特定のフォルダ以下のものは常に除外するようにする

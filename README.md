# SqliteをUnity上で簡単に扱う


概要 - OverView -

・このライブラリを導入することでSqlite文の自動生成と実行を行います。

・このライブラリは[SQLiteUnityKit] (https://github.com/Busta117/SQLiteUnityKit)をベースにして作成されています。

## 説明  - Description -

・テーブルに合わせたクラスを作成することでセレクト、登録、更新、削除に関するSqlite文の実行をサポートします。

・他にも番号の採番、ユニークデータ存在チェックを行う関数が用意されています。データを登録する際に使用してみてください。

・手動で作成したSqlite文を実行する場合は、複雑なSqlte文（サブクエリ等）を実行する場合のみ使用することをお勧めします。

※注意

・テーブル操作系（テーブル作成、テーブル削除、列追加等）のSqlite文は一切サポートしておりません。

## 必要条件 Requirements

・このライブラリを使用するには[SQLiteUnityKit](https://github.com/Busta117/SQLiteUnityKit)に関する知識が必要になります。

・[SQLiteUnityKit](https://github.com/Busta117/SQLiteUnityKit)の使い方についてはこちらのページを参照してください。[UnityでSQLiteを扱う方法](http://qiita.com/hiroyuki7/items/5335e391c9ed397aee50)

## 使い方 - Usage -

・[使い方に関する記事](http://yunomichawan.hatenablog.com/)を作成しましたのでこちらを参照してください。

・このプロジェクト内にサンプルとなるソースファイルとデータベースが配置されています。
#### サンプルファイル一覧

・Sample.db 				… データベースファイル。サンプルとなるテーブルが作成されています。

・CharacterData.cs 			… データベースの「CharacterTable」を基に作成されたクラス。

・MasterTable.cs 			… データベースの「MasterTable」を基に作成されたクラス。

・PartyCharacterTable.cs 	… データベースの「PartyCharacterTable」を基に作成されたクラス。

・PartyTable.cs 			… データベースの「PartyTable」を基に作成されたクラス。

・SqliteTableService.cs 	… データベース操作をまとめたファイル。

・SqliteTestScript.cs 		… Unity上で動作確認をするためのファイル。

## ライセンス - Licence -

・[MIT](https://github.com/yunomichawan/ConvenientSqliteForUnity/blob/master/LICENSE)

・MITライセンスとは？という方はこちらのページを参照してください。[MITライセンスについて](http://wisdommingle.com/mit-license/)

## 作成者 - Author -

湯呑み茶碗 - yunomichawan - 
[開発ブログ](http://yunomichawan.hatenablog.com/)

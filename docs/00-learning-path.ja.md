<!-- i18n: language-switcher -->
[English](00-learning-path.md) | [日本語](00-learning-path.ja.md)

# 16週間ロードマップ

週5日、1日60〜90分を想定します。速く終わってもRecallとTransferを省略しません。

## 進捗状況

- [ ] モジュール0：Nix、VS Code、FSI
- [ ] モジュール1：関数とパイプライン
- [ ] モジュール2：代数的データ型
- [ ] モジュール3：違法な状態を表現不能にする
- [ ] モジュール4：Resultとレールウェイ指向プログラミング
- [ ] モジュール5：ドメインワークフロー
- [ ] モジュール6：関数型コア / 命令型シェル
- [ ] モジュール7：境界と永続化
- [ ] モジュール8：性質ベースの思考
- [ ] モジュール9：関数型設計の代替案
- [ ] モジュール10：Parkingの境界付け
- [ ] モジュール11：解説とポートフォリオ
- [ ] モジュール12：Elmishモデル/更新/ビュー
- [ ] モジュール13：FableとJavaScriptの相互運用
- [ ] モジュール14：型安全なクライアント/サーバー境界
- [ ] モジュール15：フルスタックParking出力

## 1〜2週目：関数的に考える

入力：

- 『F# for Fun and Profit』の「Why use F#」「Thinking Functionally」
- `labs/00-fsharp-basics.fsx`
- `labs/01-types-and-functions.fsx`

出力：

- `map`、`bind`、合成、部分適用の例を各1つ書く
- `CustomerId`を`Guid`のまま渡す危険性を説明する
- Java/C#のサービスクラスをtypes + functionsに書き換える

## 3〜4週目：ドメインを型でモデル化

入力：

- 『Domain Modeling Made Functional』のドメイン理解、型駆動設計、違法状態の章
- DmmfWorkshop Day 1
- `src/FSharpLab.Domain/Subscription/Types.fs`

出力：

- ANDはレコード、ORは識別型の合併型で表す
- primitive obsessionをsingle-case unionで減らす
- `Unvalidated -> Validated`の状態遷移を型で表現
- Parkingの公開状態をDUで設計する

## 5〜6週目：レールウェイとワークフロー

入力：

- Railway-Oriented-Programming-Example
- `labs/02-result-pipeline.fsx`
- `src/FSharpLab.Domain/Subscription/Upgrade.fs`

出力：

- `Result.bind`を自作実装する
- validationエラーと予期しない例外の違いを説明する
- 純粋なDeriverをテーブル駆動テストする
- 同じパイプラインをParking料金検証に転用する

## 7〜8週目：関数型アーキテクチャ

入力：

- DmmfWorkshop Day 2
- Antmanの関数型コア / 命令型シェル
- `src/FSharpLab.Application/Subscription/UpgradeSubscription.fs`

出力：

- I/Oをコントローラーに押し出す
- 関数のパラメータによる依存性注入を実装する
- リポジトリのインメモリーアダプターを差し替える
- ビジネス結果とシステム障害の扱いを比較する

## 9〜10週目：設計の深さ

入力：

- PropertyBasedTesting
- 『13 ways of looking at a turtle』
- 『DesigningWithCapabilities』
- pipeline_oriented_programming_talk

出力：

- 例に基づくテストを性質に言い換える
- capabilityを関数型の制限として設計する
- 同じ問題を3つの設計で実装し、トレードオフを書く

## 11〜12週目：伝達と解説

入力：

- `labs/parking/README.md`
- Scott Wlaschinの主要リポジトリ一覧

出力：

- Parkingの境界付けされたコンテキストを最初から実装する
- 10個以上のドメインテストを作成する
- コンテキストマップとワークフローダイアグラムを書く
- 2分の説明と10分のディープダイブを録音する
- READMEに設計判断とトレードオフを追記する

## 完了判定

次の質問に資料なしで答えられることを完了条件とします。

1. なぜ型はドキュメント以上の役割を持つのか
2. レコードと識別型合併型をどう使い分けるか
3. validationを`bool`、`option`、`Result`のどれで返すか
4. 純粋なドメイン関数からI/Oをどう分離するか
5. `bind`はパイプラインで何を解決するか
6. ドメインエラーとシステムエラーはどう違うか
7. bounded context間で同名の型を共有しない理由は何か
8. ワークフローの入力、出力、依存性を型署名で説明できるか

## 13〜16週目：Zaid Ajaj / Fable / Elmish

入力：

- 『The Elmish Book』
- Fable.Remoting
- `labs/03-elmish-update.fsx`

出力：

- `Model * Msg -> Model * Cmd<Msg>`を説明する
- Loading/Loaded/FailedをDUで表す
- 共有APIを関数のレコードとして定義する
- FDDDのParkingワークフローをFable UIから呼び出す
- HTTP/JSONの失敗をUI状態へ明示的に写像する
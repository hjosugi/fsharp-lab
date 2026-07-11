<!-- i18n: language-switcher -->
[English](03-functional-core-imperative-shell.md) | [日本語](03-functional-core-imperative-shell.ja.md)

# Functional Core / Imperative Shell

このlabではAntmanの記事の用語を、F#とScott Wlaschinのworkflow設計へ接続します。

## Functional Core

- Entity: record、DU、single-case union
- Invariant: 一つのbusiness ruleを判定するpure function
- Deriver: 必要なdataをすべて受け、changeまたはdomain outcomeを返すpure function
- 同じ入力には同じ出力
- DB、clock、random、networkを直接呼ばない

## Imperative Shell

- Controller: data取得、Deriver呼び出し、結果に応じたeffect実行
- Repository: DB representationとdomain typeの変換境界
- Adapter: payment、email、queue、HTTPなどの具体実装
- Composition Root: dependencyを組み立てる唯一の場所

## 判断基準

「この条件で承認できるか」はCoreです。「承認結果をDBへ保存する」はShellです。「DB接続が失敗した」はdomain outcomeではなくsystem failureです。

## Output

`UpgradeSubscriptionController.create`のdependencyを一つずつfakeに差し替え、各outcomeで呼ばれるeffectだけをtestします。Deriverのbusiness ruleはController testで重複テストしません。

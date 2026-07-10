# 16週間ロードマップ

週5日、1日60〜90分を想定します。速く終わってもRecallとTransferを省略しません。

## Progress

- [ ] Module 0: Nix、VS Code、FSI
- [ ] Module 1: Functions and pipelines
- [ ] Module 2: Algebraic data types
- [ ] Module 3: Making illegal states unrepresentable
- [ ] Module 4: Result and Railway Oriented Programming
- [ ] Module 5: Domain workflows
- [ ] Module 6: Functional Core / Imperative Shell
- [ ] Module 7: Boundaries and persistence
- [ ] Module 8: Property-based thinking
- [ ] Module 9: Functional design alternatives
- [ ] Module 10: Parking bounded context
- [ ] Module 11: Explanation and portfolio
- [ ] Module 12: Elmish model/update/view
- [ ] Module 13: Fable and JavaScript interop
- [ ] Module 14: Type-safe client/server boundary
- [ ] Module 15: Full-stack Parking output

## Weeks 1–2: Think functionally

Input:

- F# for Fun and Profitの「Why use F#」「Thinking Functionally」
- `labs/00-fsharp-basics.fsx`
- `labs/01-types-and-functions.fsx`

Output:

- `map`、`bind`、composition、partial applicationを各1例書く
- `CustomerId`を`Guid`のまま渡す危険を説明する
- Java/C#のservice classをtypes + functionsへ書き換える

## Weeks 3–4: Model the domain with types

Input:

- Domain Modeling Made Functionalのdomain理解、type-driven design、illegal statesの章
- DmmfWorkshop Day 1
- `src/FSharpLab.Domain/Subscription/Types.fs`

Output:

- ANDはrecord、ORはdiscriminated unionで表す
- primitive obsessionをsingle-case unionで減らす
- `Unvalidated -> Validated`の状態遷移を型で表す
- Parkingの公開状態をDUで設計する

## Weeks 5–6: Railway and workflows

Input:

- Railway-Oriented-Programming-Example
- `labs/02-result-pipeline.fsx`
- `src/FSharpLab.Domain/Subscription/Upgrade.fs`

Output:

- `Result.bind`を自分で実装する
- validation errorとunexpected exceptionの違いを説明する
- pureなDeriverをtable-driven testする
- 同じpipelineをParking料金検証へ転用する

## Weeks 7–8: Functional architecture

Input:

- DmmfWorkshop Day 2
- AntmanのFunctional Core / Imperative Shell
- `src/FSharpLab.Application/Subscription/UpgradeSubscription.fs`

Output:

- I/OをControllerへ押し出す
- function parameterによるdependency injectionを実装する
- Repositoryのin-memory adapterを差し替える
- business outcomeとsystem failureの扱いを比較する

## Weeks 9–10: Design depth

Input:

- PropertyBasedTesting
- 13 ways of looking at a turtle
- DesigningWithCapabilities
- pipeline_oriented_programming_talk

Output:

- example-based testをpropertyへ言い換える
- capabilityをfunction typeとして制限する
- 同じ問題を3つの設計で実装してtradeoffを書く

## Weeks 11–12: Transfer and explain

Input:

- `labs/parking/README.md`
- Scott Wlaschin主要リポジトリ一覧

Output:

- Parking bounded contextを最初から実装する
- 10個以上のdomain testを作る
- context mapとworkflow diagramを書く
- 2分説明と10分deep diveを録音する
- READMEへ設計判断とtradeoffを追記する

## 完了判定

次の質問に資料なしで答えられることを完了条件にします。

1. なぜ型はdocumentation以上の役割を持つのか
2. recordとdiscriminated unionをどう使い分けるか
3. validationを`bool`、`option`、`Result`のどれで返すか
4. pure domain functionからI/Oをどう分離するか
5. `bind`はpipelineで何を解決するか
6. domain errorとsystem errorはどう違うか
7. bounded context間で同名の型を共有しない理由は何か
8. workflowの入力、出力、dependencyをtype signatureで説明できるか

## Weeks 13–16: Zaid Ajaj / Fable / Elmish

Input:

- The Elmish Book
- Fable.Remoting
- `labs/03-elmish-update.fsx`

Output:

- `Model * Msg -> Model * Cmd<Msg>`を説明する
- Loading/Loaded/FailedをDUで表す
- shared APIをrecord of functionsとして定義する
- FDDDのParking workflowをFable UIから呼び出す
- HTTP/JSONのfailureをUI stateへ明示的に写像する

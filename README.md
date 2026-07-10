# F# Lab

F#を「構文暗記」で終わらせず、Scott Wlaschinの考え方をInput、実装、説明、別ドメインへの再適用まで繰り返して身につける学習環境です。

主軸は次の3つです。

1. [F# for Fun and Profit](https://fsharpforfunandprofit.com/)で関数型の考え方を学ぶ
2. [Domain Modeling Made Functional](https://pragprog.com/titles/swdddf/domain-modeling-made-functional/)と[公開サンプル](https://github.com/swlaschin/DomainModelingMadeFunctional)で型駆動のDDDを学ぶ
3. [Railway-Oriented-Programming-Example](https://github.com/swlaschin/Railway-Oriented-Programming-Example)でエラーを型とpipelineに組み込む

補助として、[DmmfWorkshop](https://github.com/swlaschin/DmmfWorkshop)と[Functional Domain Driven Design: Simplified](https://antman-does-software.com/functional-domain-driven-design-simplified)のFunctional Core / Imperative Shellも扱います。

## 5分で開始

前提はNixとVS Codeだけです。

```bash
git clone https://github.com/hjosugi/fsharp-lab.git
cd fsharp-lab
nix develop
code .
```

VS Codeが推奨拡張を表示したらIonideをインストールします。その後、統合ターミナルで次を実行します。

```bash
just check
just run
```

最初の小さなスクリプトは次で実行できます。

```bash
just basics
```

## Input → Outputループ

各Moduleは、以下を全部終えて初めて完了です。

| 段階 | 必須成果物 |
|---|---|
| Input | 指定記事・章を読み、自分の言葉で5行に要約する |
| Recall | 資料を閉じ、主要な型とpipelineを紙またはMarkdownに再現する |
| Implement | `labs/`の課題をF#で実装する |
| Verify | example、edge case、失敗ケースのテストを追加する |
| Explain | 2分で「なぜこの型にしたか」を説明する |
| Transfer | SubscriptionではなくParkingドメインで同じ考えを再実装する |

進捗管理は[16週間ロードマップ](docs/00-learning-path.md)を使います。

## 完成サンプル

`src/`にはSubscription upgradeを題材にした、最小の実行可能なFunctional DDDがあります。

```mermaid
flowchart TD
    A["CLI / inbound adapter"] --> B["Controller / imperative shell"]
    B --> C["Deriver / functional core"]
    B --> D["Repository ports"]
    B --> E["Payment and notification ports"]
    D --> F["In-memory adapters"]
    E --> G["Console adapters"]
```

- Entity: F# recordとsingle-case union
- Invariant: 小さなpure function
- Deriver: 入力からdiscriminated unionの結果を返すpure function
- Controller: Repositoryや外部I/Oを調整する`Async` function
- Adapter: In-memory Repositoryとconsole effect

## ディレクトリ

```text
labs/                         FSIで動かす段階的な演習
labs/parking/                 別ドメインへ転用する最終課題
src/FSharpLab.Domain/         型、Invariant、Deriver
src/FSharpLab.Application/    Port、Controller、workflow
src/FSharpLab.Infrastructure/ Repositoryとeffect adapter
src/FSharpLab.Cli/            Composition Rootと実行例
tests/FSharpLab.Tests/         外部test package不要の高速test runner
docs/                         読書順、理解基準、設計メモ
```

## コマンド

| Command | Purpose |
|---|---|
| `just basics` | 最初のF# scriptを実行 |
| `just build` | 全projectをbuild |
| `just test` | DomainとControllerのtestを実行 |
| `just run` | 完成サンプルを実行 |
| `just parking-solution` | Parking課題の解答例を実行 |
| `just check` | buildとtestをまとめて実行 |

## 技術基準

- .NET 10 LTS / `net10.0`
- F# SDK-style projects
- Nix flake (`nixpkgs` unstableの`dotnet-sdk_10`)
- VS Code + Ionide
- Node.js 24（Fable/Elmish発展track）
- NuGet依存なしのcore sample
- Linux、macOS、NixOS、WSL上のNixを対象

Scott Wlaschinのcoreを終えた後は、[Fable/Elmish発展track](docs/08-fable-elmish-track.md)でZaid Ajajの教材へ進みます。

まず[Module 0](docs/00-learning-path.md)を開始してください。

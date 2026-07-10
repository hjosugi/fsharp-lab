# Scott Wlaschin学習マップ

GitHubの全公開リポジトリを確認し、学習価値で分類しています。古いruntimeを対象にしたrepositoryは、そのままbuildするより概念を現行.NETへ再実装します。

## Tier 1: 必須

| Resource | Input | Required output |
|---|---|---|
| [fsharpforfunandprofit.com](https://github.com/swlaschin/fsharpforfunandprofit.com) | thinking functionally、types、composition | 主要記事の5行要約とFSI再実装 |
| [DomainModelingMadeFunctional](https://github.com/swlaschin/DomainModelingMadeFunctional) | Order Taking workflow | 別domainのworkflowを型から設計 |
| [DmmfWorkshop](https://github.com/swlaschin/DmmfWorkshop) | 2日分のhands-on | 全exerciseと振り返り |
| [Railway-Oriented-Programming-Example](https://github.com/swlaschin/Railway-Oriented-Programming-Example) | Result pipeline | bind/map/mapErrorを再実装 |
| [RailwayOrientedProgramming](https://github.com/swlaschin/RailwayOrientedProgramming) | slidesと背景 | 5分説明とerror model比較 |

## Tier 2: 設計を深める

| Resource | Focus |
|---|---|
| [13-ways-of-looking-at-a-turtle](https://github.com/swlaschin/13-ways-of-looking-at-a-turtle) | 同じproblemに対する複数design |
| [pipeline_oriented_programming_talk](https://github.com/swlaschin/pipeline_oriented_programming_talk) | pipeline-oriented design |
| [PropertyBasedTesting](https://github.com/swlaschin/PropertyBasedTesting) | exampleからpropertyへ |
| [PropBasedTestingTalk](https://github.com/swlaschin/PropBasedTestingTalk) | property-based testingの説明 |
| [DesigningWithCapabilities](https://github.com/swlaschin/DesigningWithCapabilities) | 必要最小限の権限をfunctionで渡す |
| [DomainModellingInFsharp](https://github.com/swlaschin/DomainModellingInFsharp) | DDD talk material |
| [OxfordDDD](https://github.com/swlaschin/OxfordDDD) | 比較的新しいDDD material |
| [low-risk-ways-to-use-fsharp-at-work](https://github.com/swlaschin/low-risk-ways-to-use-fsharp-at-work) | 実務への段階導入 |
| [RefactoringFSharp](https://github.com/swlaschin/RefactoringFSharp) | F# refactoring |
| [fsharp-decompiled](https://github.com/swlaschin/fsharp-decompiled) | F#が.NETへどう表現されるか |

## Tier 3: 補助・歴史資料

- DDDEU_DMMF、LNDDD2019、DmmfWorkshop_2hr、DmmfWorkshop_8hr
- TechTrain2021、dotnetconf2021、NDC_London_2013
- turtle、Microwave、Chessie
- fsharpforfunandprofit.com_code、fsharpforfunandprofit.gitbook

## 対象外または必要時のみ

FSharp.Formatting、MBrace.StarterKit、website mirror、空repository、古いinfrastructure sampleは、F#/FDDDの中核習得後に必要に応じて確認します。

## InputをOutputへ変換するテンプレート

各repositoryごとに次のMarkdownを自分で追加します。

```markdown
# Resource name

## Five-line summary
## Three type signatures to remember
## One idea I disagree with
## Reimplementation without looking
## Tests and edge cases
## Transfer to the Parking domain
## Two-minute explanation
```


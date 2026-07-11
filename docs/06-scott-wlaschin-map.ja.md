<!-- i18n: language-switcher -->
[English](06-scott-wlaschin-map.md) | [日本語](06-scott-wlaschin-map.ja.md)

# Scott Wlaschin学習マップ

GitHubの全公開リポジトリを確認し、学習価値で分類しています。古いランタイムを対象にしたリポジトリは、そのままビルドするより概念を現行.NETへ再実装します。

## Tier 1: 必須

| リソース | 入力 | 必須出力 |
|---|---|---|
| [fsharpforfunandprofit.com](https://github.com/swlaschin/fsharpforfunandprofit.com) | 関数型思考、型、合成 | 主要記事の5行要約とFSI再実装 |
| [DomainModelingMadeFunctional](https://github.com/swlaschin/DomainModelingMadeFunctional) | 注文受付ワークフロー | 別ドメインのワークフローを型から設計 |
| [DmmfWorkshop](https://github.com/swlaschin/DmmfWorkshop) | 2日分のハンズオン | 全演習と振り返り |
| [Railway-Oriented-Programming-Example](https://github.com/swlaschin/Railway-Oriented-Programming-Example) | Resultパイプライン | bind/map/mapErrorを再実装 |
| [RailwayOrientedProgramming](https://github.com/swlaschin/RailwayOrientedProgramming) | スライドと背景 | 5分説明とエラーモデル比較 |

## Tier 2: 設計を深める

| リソース | フォーカス |
|---|---|
| [13-ways-of-looking-at-a-turtle](https://github.com/swlaschin/13-ways-of-looking-at-a-turtle) | 同じ問題に対する複数の設計 |
| [pipeline_oriented_programming_talk](https://github.com/swlaschin/pipeline_oriented_programming_talk) | パイプライン指向設計 |
| [PropertyBasedTesting](https://github.com/swlaschin/PropertyBasedTesting) | 例からプロパティへ |
| [PropBasedTestingTalk](https://github.com/swlaschin/PropBasedTestingTalk) | プロパティベースのテスト解説 |
| [DesigningWithCapabilities](https://github.com/swlaschin/DesigningWithCapabilities) | 必要最小限の権限を関数で渡す |
| [DomainModellingInFsharp](https://github.com/swlaschin/DomainModellingInFsharp) | DDDトーク資料 |
| [OxfordDDD](https://github.com/swlaschin/OxfordDDD) | 比較的新しいDDD資料 |
| [low-risk-ways-to-use-fsharp-at-work](https://github.com/swlaschin/low-risk-ways-to-use-fsharp-at-work) | 実務への段階的導入 |
| [RefactoringFSharp](https://github.com/swlaschin/RefactoringFSharp) | F#リファクタリング |
| [fsharp-decompiled](https://github.com/swlaschin/fsharp-decompiled) | F#が.NETにどう表現されるか |

## Tier 3: 補助・歴史資料

- DDDEU_DMMF、LNDDD2019、DmmfWorkshop_2hr、DmmfWorkshop_8hr
- TechTrain2021、dotnetconf2021、NDC_London_2013
- turtle、Microwave、Chessie
- fsharpforfunandprofit.com_code、fsharpforfunandprofit.gitbook

## 対象外または必要時のみ

FSharp.Formatting、MBrace.StarterKit、ウェブサイトミラー、空リポジトリ、古いインフラサンプルは、F#/FDDDの中核習得後に必要に応じて確認します。

## InputをOutputへ変換するテンプレート

各リポジトリごとに次のMarkdownを自分で追加します。

```markdown
# リソース名

## 五行要約
## 記憶すべき三つの型シグネチャ
## 一つの意見に反対
## 見ずに再実装
## テストとエッジケース
## パーキングドメインへの移行
## 二分間の説明
```
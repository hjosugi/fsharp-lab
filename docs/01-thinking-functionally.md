# Thinking functionally

## 覚える軸

- Objectより先にdata transformationを見る
- mutationより新しい値を返す
- statementの列よりfunction pipelineを見る
- dependencyをhidden stateではなくparameterにする
- type signatureを実装前の設計として使う

## Input

1. F# for Fun and ProfitのWhy use F# series
2. Functions、partial application、composition、pipelineの各series
3. `labs/00-fsharp-basics.fsx`

## Output

次を資料なしで実装します。

```fsharp
val map : ('a -> 'b) -> 'a list -> 'b list
val bind : ('a -> Result<'b, 'e>) -> Result<'a, 'e> -> Result<'b, 'e>
val compose : ('a -> 'b) -> ('b -> 'c) -> ('a -> 'c)
```

説明では「関数を小さくすること」ではなく、「入力と出力を明示し、合成可能にすること」を中心にします。


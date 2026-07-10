# Railway Oriented Programming

Railway Oriented Programmingは、成功/失敗するfunctionを直線的なpipelineに合成する考え方です。

## 最小モデル

```fsharp
let bind switchFunction twoTrackInput =
    match twoTrackInput with
    | Ok value -> switchFunction value
    | Error error -> Error error
```

## 使う場面

- 入力parse
- Value Object生成
- 複数validation
- business workflowのexpected failure

## 使わない場面

- OutOfMemoryなど回復不能なruntime failure
- DB driver bugをbusiness errorに見せかけること
- 何でも同じ`string` errorへ潰すこと

## Output

1. `labs/02-result-pipeline.fsx`を実行する
2. `bind`、`map`、`mapError`を資料なしで書く
3. errorをDUへ変更する
4. errorを一件ずつ返す設計と、全件蓄積するvalidation設計を比較する


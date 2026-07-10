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

## Boundary errorも型で分ける

`PaymentGateway`は自由な`string`ではなく、呼び出し側が処理できる失敗だけをDUで返します。

```fsharp
type PaymentError =
    | Declined of reason: string
    | GatewayUnavailable

type PaymentGateway = {
    Charge: CardToken -> Money -> Async<Result<TransactionId, PaymentError>>
}
```

`Declined`や既知の一時的な停止はexpected failureです。一方、programming errorや壊れたinvariantまで`PaymentError`へ変換すると原因を隠します。adapterは既知の失敗だけをmapし、unexpected exceptionはloggingと最外層のerror handlingへ渡します。

## Output

1. `labs/02-result-pipeline.fsx`を実行する
2. `bind`、`map`、`mapError`を資料なしで書く
3. errorをDUへ変更する
4. errorを一件ずつ返す設計と、全件蓄積するvalidation設計を比較する

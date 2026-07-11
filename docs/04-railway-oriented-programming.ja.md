<!-- i18n: language-switcher -->
[English](04-railway-oriented-programming.md) | [日本語](04-railway-oriented-programming.ja.md)

# 鉄道型プログラミング

鉄道型プログラミングは、成功/失敗する関数を直線的なパイプラインに合成する考え方です。

## 最小モデル

```fsharp
let bind switchFunction twoTrackInput =
    match twoTrackInput with
    | Ok value -> switchFunction value
    | Error error -> Error error
```

## 使う場面

- 入力のパース
- 値オブジェクトの生成
- 複数のバリデーション
- ビジネスワークフローの予期される失敗

## 使わない場面

- OutOfMemoryなどの回復不能なランタイム失敗
- データベースドライバのバグをビジネスエラーに見せかけること
- 何でも同じ`string`エラーに潰すこと

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

`Declined`や既知の一時的な停止は予期される失敗です。一方、プログラミングエラーや壊れたインバリアントまで`PaymentError`へ変換すると原因を隠してしまいます。アダプターは既知の失敗だけをマップし、予期しない例外はロギングと最外層のエラーハンドリングへ渡します。

## 出力

1. `labs/02-result-pipeline.fsx`を実行する
2. `bind`、`map`、`mapError`を資料なしで書く
3. エラーをDUへ変更する
4. エラーを一件ずつ返す設計と、全件蓄積するバリデーション設計を比較する
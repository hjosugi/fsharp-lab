<!-- i18n: language-switcher -->
[English](01-thinking-functionally.md) | [日本語](01-thinking-functionally.ja.md)

# 関数型的思考

## 覚えるべきポイント

- オブジェクトよりも先にデータ変換を見る
- ミューテーションよりも新しい値を返す
- ステートメントの列よりも関数のパイプラインを見る
- 依存関係を隠れた状態ではなくパラメータにする
- 型シグネチャを実装前の設計として活用する

## 入力

1. F# for Fun and Profitの「Why use F#」シリーズ
2. 関数、部分適用、合成、パイプラインの各シリーズ
3. `labs/00-fsharp-basics.fsx`

## 出力

資料なしで次の関数を実装します。

```fsharp
val map : ('a -> 'b) -> 'a list -> 'b list
val bind : ('a -> Result<'b, 'e>) -> Result<'a, 'e> -> Result<'b, 'e>
val compose : ('a -> 'b) -> ('b -> 'c) -> ('a -> 'c)
```

説明では「関数を小さくすること」ではなく、「入力と出力を明示し、合成可能にすること」を中心にします。
# Domain modeling with types

## 基本対応

| Domain concept | F# representation |
|---|---|
| 複数項目をすべて持つ | Record（AND type） |
| 複数状態のどれか | Discriminated union（OR type） |
| 制約付きprimitive | Private single-case union + smart constructor |
| 状態遷移 | 入力状態と出力状態が異なるfunction |
| Business operation | Named function type |
| Expected failure | `Result`またはdomain-specific union |

## 重要原則

`string`を受け取って毎回validationするのではなく、validation済みの型を作り、その型を受け取るfunctionでは再検証を不要にします。

ただし、外部JSON、DB row、CLI引数は信用できません。境界でparseし、domain typeへ変換します。これは「Parse, don't validate」と同じ方向です。

## Output

- `CustomerId`、`Money`、`CardToken`のsmart constructorを書く
- `ParkingPublicationState`をDUにする
- `Closed`なのに`Published`でもある状態を表現不能にする
- 型だけを見てworkflowを説明する


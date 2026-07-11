<!-- i18n: language-switcher -->
[English](07-output-rubric.en.md) | [日本語](07-output-rubric.md)

# Output rubric

各Moduleを0〜3で自己評価します。

| Score | Meaning |
|---:|---|
| 0 | 読んでいない、動かしていない |
| 1 | 見ながら実装できる |
| 2 | 資料なしで実装・説明できる |
| 3 | 別domainへ転用し、tradeoffを説明できる |

## 必須check

- [ ] Type signatureだけで入力・出力・失敗を説明できる
- [ ] Happy path以外をDUで列挙できる
- [ ] Pure coreをmockなしでtestできる
- [ ] I/O dependencyをfunction/recordで差し替えられる
- [ ] 不正状態を作るcodeがcompileしない理由を説明できる
- [ ] Alternative designを一つ示せる
- [ ] Parking domainへ移植できる

合格はすべてScore 2以上、Module 4〜10はScore 3です。


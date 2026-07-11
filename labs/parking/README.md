<!-- i18n: language-switcher -->
[English](README.en.md) | [日本語](README.md)

# Parking bounded context challenge

Subscription sampleを見ずに、同じ設計をParkingへ転用します。

## Domain

駐車場はDraftとして登録されます。名前、住所、1時間料金、営業時間が揃うと公開申請できます。Reviewerが承認するとPublishedになります。Closedになった駐車場は再公開できません。

## Part 1: Types

- `ParkingId`、`HourlyRate`、`Address`をprimitiveのままにしない
- `Draft`、`PendingApproval`、`Published`、`Closed`をboolean flagsで表さない
- 公開済み日時がDraftに存在する設計を避ける

## Part 2: Invariants

- 名前は空でない
- 料金は0以上
- 開始時刻は終了時刻より前
- 必須項目が揃っている

## Part 3: Deriver

`requestPublication`はpure functionにします。DB、clock、loggerを直接呼びません。必要な時刻は引数で受け取ります。

Expected outcomesをDUで列挙してください。

- RequestAccepted
- AlreadyPending
- AlreadyPublished
- ParkingIsClosed
- MissingRequiredFields

## Part 4: Controller

- RepositoryからParkingを取得
- pure Deriverを呼ぶ
- acceptedなら保存してreview eventをpublish
- missingならNotFoundを返す

## Part 5: Tests

最低10ケースです。happy pathだけでなく、各union case、境界値、effectが呼ばれないcaseを含めます。

`Starter.fsx`から始め、最後にだけ`Solution.fsx`と比較してください。解答例と異なっても、型がruleを守りtradeoffを説明できれば正解です。


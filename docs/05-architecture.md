# Architecture and tradeoffs

## Dependency direction

DomainはApplication、Infrastructure、CLIを参照しません。ApplicationはDomainだけを参照します。Infrastructureがportを実装し、CLIが全部を組み立てます。

## Transaction boundary

単一DB内で複数writeをatomicにする必要がある場合、transaction capabilityをApplication portとして渡し、Controllerがoperation boundaryを決めます。

PaymentとDBのような分散effectは通常のDB transactionだけではatomicになりません。次の設計を比較します。

- Paymentを先に行い、save失敗時にrefund/compensationする
- Saveを先に行い、outboxからpaymentを非同期実行する
- Saga/process managerで進捗をdomain stateとして持つ

このlabのsampleは小ささを優先し、Payment成功後にin-memory saveします。本番設計ではoutbox、idempotency key、retry、compensationが必要です。

## Credential boundary

Payment adapterは処理に`CardToken`を必要としますが、loggingやdomain outcomeへraw valueを流しません。sampleのconsole adapterもtokenを`[redacted]`として表示します。

本番adapterでは、credentialを外部providerへ渡す箇所をboundary内に限定し、structured log、exception message、metric labelへ含めないでください。診断にはtransaction IDやprovider側のsafeなcorrelation IDを使います。

## Folder strategy

上位はbounded context/機能で分け、内部はDomain/Application/Infrastructureのdependency ruleを維持します。共通Entityを無理に共有しません。同じ「Customer」でもBillingとSupportでは必要な意味が違います。

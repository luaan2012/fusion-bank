# FusionBank

## Projeto para simular um banco com funcionalidades parecidades, um projeto que visa ser robusto em termos de técnologias e escrito com .NET e no frontEnd Angular

- api-bank-account
 responsavel por criar conta, excluir conta, atualizar dados, consultar dados

- api-bank-transfer
 responsavel por transferencias, consulta api-bank-central, ted, doc e pix

- api-bank-deposit
 responsavel por depositos nas contas simulando um deposito

- api-bank-notification
 responsavel por notificar qualquer ação ou evento que ocorrer através de filas rabbitMQ/krafka

- api-bank-investiments
 responsavel por simular CDBS e outros investimentos

- api-bank-central
 responsavel por ser o centro de todas as transacoes, guardar historico de movimentacoes, criacao de bancos, criacao de contas, liberacao de transferencia

namespace fusion.bank.core.Messages.Producers
{
    public record NewDepositAccountProducer(Guid AccountId, Guid DepositId, string AccountNumberReceiver, string AgencyNumberReceiver, decimal Amount, string Desscription);
}

namespace fusion.bank.core.Messages.Producers
{
    public record NewDepositAccountProducer(Guid AccountId, Guid DepositId, string AccountNumber, decimal Amount);
}

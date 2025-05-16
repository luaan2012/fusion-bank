namespace fusion.bank.core.Messages.Producers
{
    public record DepositedAccountProducer(Guid DepositId, Guid AccountId, Guid? AccountIdReceiver, bool Deposited);
}

namespace fusion.bank.core.Messages.Consumers
{
    public record DepositedAccountConsumer(Guid DepositId, bool Deposited);
}

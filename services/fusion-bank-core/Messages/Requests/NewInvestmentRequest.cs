namespace fusion.bank.core.Messages.Requests
{
    public record NewInvestmentRequest(Guid AccountId, decimal Amount);
}

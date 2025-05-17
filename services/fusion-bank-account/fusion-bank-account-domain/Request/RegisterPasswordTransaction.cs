namespace fusion.bank.account.domain.Request
{
    public record RegisterPasswordTransaction(Guid AccountId, string PasswordTransaction);
}

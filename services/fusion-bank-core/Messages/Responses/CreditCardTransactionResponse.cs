namespace fusion.bank.core.Messages.Responses
{
    public class CreditCardTransactionResponse(bool success)
    {
        public bool Success { get; set; } = success;
    }
}

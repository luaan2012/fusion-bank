namespace fusion.bank.core.Messages.Responses
{
    public class TransferredAccountResponse(bool transferred, Guid accountId, string nameReceiver)
    {
        public bool Transferred { get; set; } = transferred;
        public Guid AccountId { get; set; } = accountId;
        public string NameReceiver { get; set; } = nameReceiver;
    }
}

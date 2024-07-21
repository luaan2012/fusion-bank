namespace fusion.bank.core.Messages.Responses
{
    public class TransferredAccountResponse
    {
        public TransferredAccountResponse(bool transferred)
        {
            Transferred = transferred;
        }

        public bool Transferred { get; set; }
    }
}

namespace fusion.bank.core.Messages.Responses
{
    public class TransferredCentralResponse
    {
        public TransferredCentralResponse(bool transferred)
        {
            Transferred = transferred;
        }

        public bool Transferred { get; set; }
    }
}

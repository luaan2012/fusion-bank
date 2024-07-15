namespace fusion.bank.core.Messages.Consumers
{
    //public record DepositedCentralConsumer(Guid DepositId, bool Deposited);

    public class DepositedCentralResponse
    {
        public DepositedCentralResponse(Guid depositId, bool deposited)
        {
            DepositId = depositId;
            Deposited = deposited;
        }

        public DepositedCentralResponse()
        {
            
        }

        public Guid DepositId { get; set; }
        public bool Deposited { get; set; }
    }
}

namespace fusion.bank.core.Messages.Requests
{
    //public record DepositedCentralConsumer(Guid DepositId, bool Deposited);

    public class DepositedCentralProducer
    {
        public DepositedCentralProducer(Guid depositId, bool deposited)
        {
            DepositId = depositId;
            Deposited = deposited;
        }

        public DepositedCentralProducer()
        {

        }

        public Guid DepositId { get; set; }
        public bool Deposited { get; set; }
    }
}

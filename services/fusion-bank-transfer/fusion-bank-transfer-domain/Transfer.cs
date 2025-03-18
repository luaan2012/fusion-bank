using fusion.bank.transfer.domain.Enum;
using MongoDB.Bson.Serialization.Attributes;
using fusion.bank.core.Enum;

namespace fusion.bank.transfer.domain
{
    public class Transfer
    {
        [BsonId]
        public Guid TransferId { get; internal set; }    
        public string AccountNumberReceive { get; set; }
        public string KeyAccount { get; set; }
        public decimal Amount { get; set; }
        public string NameOwner { get; set; }
        public string NameReceive { get; set; }
        public string DocumentReceive { get; set; }
        public string DocumentOwner { get; set; }
        public string AccountNumberOwner { get; set; }
        public bool IsSchedule { get; set; }
        public DateTime ScheduleDate { get; set; }
        public TransferStatus TransferStatus { get; set; }
        public TransferType TransferType { get; set; }
    }
}

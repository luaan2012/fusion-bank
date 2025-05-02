using fusion.bank.transfer.domain.Enum;
using MongoDB.Bson.Serialization.Attributes;
using fusion.bank.core.Enum;
using MongoDB.Bson;

namespace fusion.bank.transfer.domain
{
    public class Transfer
    {
        [BsonId]
        public Guid TransferId { get; internal set; }    
        public Guid AccountId { get; internal set; }
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

        [BsonRepresentation(BsonType.String)]
        public TransferStatus TransferStatus { get; set; }

        [BsonRepresentation(BsonType.String)]
        public TransferType TransferType { get; set; }
    }
}

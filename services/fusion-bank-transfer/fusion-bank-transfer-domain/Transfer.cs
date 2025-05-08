using fusion.bank.transfer.domain.Enum;
using MongoDB.Bson.Serialization.Attributes;
using fusion.bank.core.Enum;
using MongoDB.Bson;
using fusion.bank.transfer.domain.Request;

namespace fusion.bank.transfer.domain
{
    public class Transfer
    {
        [BsonId]
        public Guid TransferId { get; internal set; }    
        public Guid AccountId { get; set; }
        public string AccountNumberReceiver { get; set; }
        public string KeyAccount { get; set; }
        public decimal Amount { get; set; }
        public string NamePayer{ get; set; }
        public string NameReceiver { get; set; }
        public string DocumentReceiver { get; set; }
        public string DocumentPayer { get; set; }
        public string AccountNumberPayer { get; set; }
        public bool IsSchedule { get; set; }
        public DateTime ScheduleDate { get; set; }

        [BsonRepresentation(BsonType.String)]
        public TransferStatus TransferStatus { get; set; }

        [BsonRepresentation(BsonType.String)]
        public TransferType TransferType { get; set; }

        public void CreateTransfer(TransferRequest request)
        {
            AccountId = request.AccountId;
            AccountNumberPayer = request.AccountNumberPayer;
            AccountNumberReceiver = request.AccountNumberReceive;
            Amount = request.Amount;
            DocumentPayer = request.DocummentPayer;
            DocumentReceiver = request.DocumentReceiver;
            IsSchedule = request.IsSchedule;
            KeyAccount = request.KeyAccount;
            NamePayer = request.NamePayer;
            NameReceiver = request.NameReceiver;
            ScheduleDate = request.ScheduleDate;
            TransferType = request.TransferType;
            TransferId = Guid.NewGuid();
            TransferStatus = TransferStatus.CREATED;
        }
    }
}

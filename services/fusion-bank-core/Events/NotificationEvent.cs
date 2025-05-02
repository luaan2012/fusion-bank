using fusion.bank.core.Enum;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace fusion.bank.core.Events
{
    public class EventMessage
    {
        public Guid EventId { get; set; } = Guid.NewGuid();
        public DateTime Date { get; set; }

        [BsonRepresentation(BsonType.String)]
        public NotificationType Action { get; set; }
        public string AccountId { get; set; }
        public string FullName { get; set; }
        public string AccountType { get; set; }
        public string UserOwner { get; set; } 
        public string UserReceive { get; set; }
        public string KeyAccount { get; set; } 
        public string AmountTransfer { get; set; }
        public string Investment { get; set; } 
        public string InvestmentAmount { get; set; } 
        public string DateSchedule { get; set; } 
        public string CreditCardType { get; set; }
        public string CreditCardNumber { get; set; } 
        public string CreditCardLimit { get; set; } 
        public string CodeGenerate { get; set; }
        public string Balance { get; set; }
        public string TransferType { get; set; }
        public string Amount { get; set; }
       
        [BsonRepresentation(BsonType.String)]
        public ServiceType Service { get; set; } 
        public string Details { get; set; } 
    }
}

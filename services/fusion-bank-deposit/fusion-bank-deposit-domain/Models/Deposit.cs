using fusion.bank.core.Helpers;
using fusion.bank.deposit.domain.Requests;
using MongoDB.Bson.Serialization.Attributes;

namespace fusion.bank.deposit.domain.Models
{
    public class Deposit
    {
        [BsonId]
        public Guid DepositId { get; set; }
        public Guid AccountId { get; set; }
        public decimal Amount { get; set; }
        public string BankSwiftCode { get; set; }
        public string CodeGenerate { get; set; }
        public string AccountNumber { get; set; }
        public DateTime DateExpiration { get; set; }
        public DateTime DateConsuption { get; set; }
        public bool Debited { get; set; }
        public bool Active { get; set; }

        public void GenerateCode(BilletRequest billetRequest)
        {
            CodeGenerate = RandomHelper.GenerateRandomNumbers(48);
            DateExpiration = DateTime.Now.AddDays(3);
            Active = true;
            BankSwiftCode = billetRequest.SwiftCode;
            Amount = billetRequest.Amount;
            AccountId = billetRequest.AccountId;
            AccountNumber = billetRequest.AccountNumber;
        }
        public void DebitedTrue()
        {
            Debited = true;
        }
        public void DisableDeposit()
        {
            Active = false;
        }

        public void ConsuptionDate()
        {
            DateConsuption = DateTime.Now;
        }
    }
}

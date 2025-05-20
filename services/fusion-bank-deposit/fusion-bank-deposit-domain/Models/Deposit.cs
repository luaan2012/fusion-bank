using fusion.bank.core.Helpers;
using fusion.bank.deposit.domain.Requests;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace fusion.bank.deposit.domain.Models
{
    public class Deposit
    {
        [BsonId]
        public Guid DepositId { get; set; }
        public Guid AccountId { get; set; }
        public decimal Amount { get; set; }
        public decimal Fee => HandleFee();
        public string ISPB { get; set; }
        public string CodeGenerate { get; set; }
        public string AccountNumber { get; set; }
        public string AgencyNumber { get; set; }
        public string Document { get; set; }
        public string Description { get; set; }

        [BsonRepresentation(BsonType.String)]
        public ExpenseCategory BilletType { get; set; }
        public DateTime DateExpiration { get; set; }
        public DateTime DateConsuption { get; set; }
        public bool Debited { get; set; }
        public bool Active { get; set; }
        public bool DirectToAccount { get; set; }

        public void GenerateCode(DepositBilletRequest billetRequest)
        {
            DepositId = Guid.NewGuid();
            CodeGenerate = RandomHelper.GenerateRandomNumbers(48);
            DateExpiration = DateTime.Now.AddDays(3);
            Active = true;
            ISPB = billetRequest.ISPB;
            Amount = billetRequest.Amount;
            AccountId = billetRequest.AccountId;
            AccountNumber = billetRequest.AccountNumberReceiver;
            AgencyNumber = billetRequest.AgencyNumberReceiver;
            Description = billetRequest.Description;
            Document = billetRequest.DocumentReceiver;
            BilletType = billetRequest.BilletType;
        }

        public void GenerateDepositDirect(DirectDeposit directDeposit)
        {
            DepositId = Guid.NewGuid();
            CodeGenerate = RandomHelper.GenerateRandomNumbers(48);
            DateExpiration = DateTime.Now.AddDays(2);
            Active = true;
            DirectToAccount = true;
            Amount = directDeposit.Amount;
            AccountId = directDeposit.AccountId;
            AccountNumber = directDeposit.AccountNumberReceive;
            AgencyNumber = directDeposit.AgencyNumberReceiver;
            Description = directDeposit.Description;
            BilletType = ExpenseCategory.DEPOSIT;    
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

        private decimal HandleFee()
        {
            var today = DateTime.Today;

            var daysLate = (today - DateExpiration.Date).Days;

            if (daysLate <= 0)
                return 0m;

            decimal feePerDay = 1.50m;
            decimal totalFee = daysLate * feePerDay;

            return totalFee;
        }
    }
}

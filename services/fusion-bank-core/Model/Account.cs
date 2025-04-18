using System.Reflection.Metadata;
using fusion.bank.core.Enum;
using fusion.bank.core.Helpers;
using fusion.bank.core.Request;
using MongoDB.Bson.Serialization.Attributes;

namespace fusion.bank.core.Model
{
    public class Account
    {
        [BsonId]
        public Guid AccountId { get; internal set; }
        public string Name { get; internal set; }
        public string LastName { get; internal set; }
        public string FullName { get; private set; }
        public string BankName { get; private set; }
        public string BankISBP { get; private set; }
        public string KeyAccount { get; private set; }
        public string AccountNumber { get; internal set; }
        public decimal Balance { get; internal set; }
        public decimal TransferLimit { get; internal set; }
        public decimal SalaryPerMonth { get; private set; }
        public List<CreditCard> CreditCards { get; set; }
        public AccountType AccountType { get; set; }
        public StatusAccount Status { get; set; }
        public DocumentType DocumentType { get; set; }
        public string Document { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public void CreateAccount(AccountRequest accountRequest)
        {
            AccountId = Guid.NewGuid();
            Name = accountRequest.Name;
            LastName = accountRequest.LastName;
            FullName = $"{accountRequest.Name} {accountRequest.LastName}";
            SalaryPerMonth = accountRequest.SalaryPerMonth;
            AccountNumber = RandomHelper.GenerateRandomNumbers(8);
            Status = StatusAccount.Pendent;
            AccountType = accountRequest.AccountType;
            BankISBP = accountRequest.BankISBP;
            DocumentType = accountRequest.DocumentType;
            Document = accountRequest.Document;
            Email = accountRequest.Email;
            Password = BCrypt.Net.BCrypt.HashPassword(accountRequest.Password);

            GetLimitAccount();
        }

        internal void GetLimitAccount()
        {
            TransferLimit = AccountType switch
            {
                AccountType.CheckingAccount => 5000 + SalaryPerMonth,
                AccountType.JointAccount => 3000 + SalaryPerMonth,
                AccountType.StudentAccount => 500,
                AccountType.SalaryAccount => 800,
                AccountType.SavingsAccount => 800,
                AccountType.BusinessAccount => 12000 + SalaryPerMonth,
            };
        }

        public void Debit(decimal amount)
        {
            if(amount <= 0)
            {
                Console.WriteLine("amount needs to be greater than 0");
            }

            if(Balance < amount)
            {
                Console.WriteLine("Insufficient funds.");
            }

            Balance -= amount;            
        }

        public void Credit(decimal amount)
        {
            if (amount <= 0)
            {
                Console.WriteLine("Amount must be greater than zero.");
            }

            Balance += amount;
        }

        public void Block()
        {
            Status = StatusAccount.Blocked;
            Console.WriteLine("Account has been blocked.");
        }

        public void Active()
        {
            Status = StatusAccount.Active;
            Console.WriteLine("Account has been blocked.");
        } 
        
        public void UpdateBankName(string bankName)
        {
            BankName = bankName;
        }
    }
}

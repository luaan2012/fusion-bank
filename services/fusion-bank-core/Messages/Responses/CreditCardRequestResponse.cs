using fusion.bank.core.Enum;

namespace fusion.bank.core.Messages.Responses
{
    public class CreditCardRequestResponse
    {
        public CreditCardRequestResponse()
        {
            
        }

        public CreditCardRequestResponse(string name, string lastName, string fullName, string bankName, string bankISBP, string keyAccount, string accountNumber, decimal balance, decimal salaryPerMonth, decimal averageBudgetPerMonth, AccountType accountType)
        {
            Name = name;
            LastName = lastName;
            FullName = fullName;
            BankName = bankName;
            BankISBP = bankISBP;
            KeyAccount = keyAccount;
            AccountNumber = accountNumber;
            Balance = balance;
            SalaryPerMonth = salaryPerMonth;
            AverageBudgetPerMonth = averageBudgetPerMonth;
            AccountType = accountType;
        }

        public string Name { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public string BankName { get; set; }
        public string BankISBP { get; set; }
        public string KeyAccount { get; set; }
        public string AccountNumber { get; set; }
        public decimal Balance { get; set; }
        public decimal SalaryPerMonth { get; set; }
        public decimal AverageBudgetPerMonth { get; set; }
        public AccountType AccountType { get; set; }
    }
}

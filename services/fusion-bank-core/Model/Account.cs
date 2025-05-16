using fusion.bank.core.Enum;
using fusion.bank.core.Helpers;
using fusion.bank.deposit.domain;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace fusion.bank.core.Model
{
    public class Account
    {
        [BsonId]
        public Guid AccountId { get; internal set; }
        public DateTime BirthDate { get; internal set; }
        public string PhoneNumber { get; internal set; }
        public string Name { get; internal set; }
        public string LastName { get; internal set; }
        public string FullName { get; private set; }
        public string BankName { get; private set; }
        public string BankISBP { get; private set; }
        public string KeyAccount { get; private set; }
        public string AccountNumber { get; internal set; }
        public string Agency { get; internal set; }
        public decimal Balance { get; internal set; }
        public decimal TransferLimit { get; internal set; }
        public decimal ExpensePerDay { get; internal set; }
        public decimal ExpenseDay { get; internal set; }
        public decimal SalaryPerMonth {  get; internal set; }
        public decimal AverageBudgetPerMonth => SumAverage();
        public List<CreditCard> CreditCards { get; set; }
        public List<ExpenseAccount> ExpenseAccounts { get; set; } = [];

        [BsonRepresentation(BsonType.String)]
        public AccountType AccountType { get; set; }

        [BsonRepresentation(BsonType.String)]
        public StatusAccount Status { get; set; }

        [BsonRepresentation(BsonType.String)]
        public DocumentType DocumentType { get; set; }

        [BsonRepresentation(BsonType.String)]
        public KeyType KeyTypePix { get; set; }
        public string Document { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public bool DarkMode { get; set; }

        public void CreateAccount(string name, string lastName, AccountType accountType, string BankIspb, DocumentType documentType, string document, string email, string password, decimal salary )
        {
            AccountId = Guid.NewGuid();
            Name = name;
            LastName = lastName;
            FullName = $"{name} {lastName}";
            SalaryPerMonth = salary;
            AccountNumber = RandomHelper.GenerateRandomNumbers(8);
            Agency = $"00{RandomHelper.GenerateRandomNumbers(2)}";
            Status = StatusAccount.PENDENT;
            AccountType = accountType;
            BankISBP = BankIspb;
            DocumentType = documentType;
            Document = document;
            Email = email;
            Password = BCrypt.Net.BCrypt.HashPassword(password);

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

        internal decimal SumAverage()
        {
            var gastosMes = ExpenseAccounts
            .GroupBy(e => new { e.Date.Year, e.Date.Month })
            .Select(g => new
            {
                Ano = g.Key.Year,
                Mes = g.Key.Month,
                TotalMes = g.Sum(e => e.Amount)
            });

            var totalGasto = gastosMes.Sum(g => g.TotalMes);
            var quantidadeMeses = gastosMes.Count();

            return quantidadeMeses > 0 ? totalGasto / quantidadeMeses : 0;
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

            ExpenseDay += amount;
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


        public void AddExpense(DateTime date, decimal amount, ExpenseCategory expenseCategory)
        {
            ExpenseAccounts.Add(new ExpenseAccount { ExpenseCategory = expenseCategory, Amount = amount, Date = date });
        }

        public void Block()
        {
            Status = StatusAccount.BLOCKED;
            Console.WriteLine("Account has been blocked.");
        }

        public void Active()
        {
            Status = StatusAccount.ACTIVE;
            Console.WriteLine("Account has been blocked.");
        } 
        
        public void UpdateBankName(string bankName)
        {
            BankName = bankName;
        }
    }

    public class ExpenseAccount
    {
        public ExpenseCategory ExpenseCategory { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
    }
}

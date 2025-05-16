using fusion.bank.core.Enum;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using fusion.bank.deposit.domain;

namespace fusion.bank.core.Model
{
    public class CreditCard
    {
        public CreditCard()
        {
            
        }

        public CreditCard(Guid id, Guid accountId, string creditCardNumber, string creditCardName, string creditCardCode, DateTime creditCardValidity, CreditCardType creditCardType, CreditCardFlag creditCardFlag, bool creditCardTried, int creditCardTriedTimes, DateTime creditCardNextAttempt, decimal creditCardLimit, decimal creditCardUsed)
        {
            Id = id;
            AccountId = accountId;
            CreditCardNumber = creditCardNumber;
            CreditCardName = creditCardName;
            CreditCardCode = creditCardCode;
            CreditCardValidity = creditCardValidity;
            CreditCardType = creditCardType;
            CreditCardFlag = creditCardFlag;
            CreditCardTried = creditCardTried;
            CreditCardTriedTimes = creditCardTriedTimes;
            CreditCardNextAttempt = creditCardNextAttempt;
            CreditCardLimit = creditCardLimit;
            CreditCardUsed = creditCardUsed;
        }

        public Guid Id { get; set; }
        public Guid AccountId { get; set; }
        public string CreditCardNumber { get; set; }
        public string CreditCardName { get; set; }
        public string CreditCardCode { get; set; }
        public decimal CreditCardLimit { get; set; }
        public decimal CreditCardUsed { get; set; }
        public decimal CreditCardAvaliable => CreditCardLimit - CurrentInvoiceAmount;
        public decimal CurrentInvoiceAmount => Invoices.FirstOrDefault(i => i.IsOpen)?.TotalAmount ?? 0;
        public List<CreditCardInvoice> Invoices { get; set; } = [];
        public DateTime CreditCardValidity { get; set; }

        [BsonRepresentation(BsonType.String)]
        public CreditCardType CreditCardType { get; set; }

        [BsonRepresentation(BsonType.String)]
        public CreditCardFlag CreditCardFlag { get; set; }
        public bool CreditCardTried { get; set; }
        public bool CreditCardBlocked { get; set; }
        public int CreditCardTriedTimes { get; set; }
        public List<VirtualCreditCard> VirtualCreditCards { get; set; } = [];
        public DateTime CreditCardNextAttempt { get; set; }

        public void AdicionarGastoComFatura(CreditCardExpense expense)
        {
            // Verifica se existe uma fatura em aberto
            var faturaAberta = Invoices.FirstOrDefault(i => i.IsOpen);

            // Se não existir, cria uma nova fatura
            if (faturaAberta == null)
            {
                faturaAberta = CriarNovaFatura();
                Invoices.Add(faturaAberta);
            }

            faturaAberta.Expenses.Add(expense);
        }

        private CreditCardInvoice CriarNovaFatura()
        {
            var agora = DateTime.Now;

            var inicioPeriodo = new DateTime(agora.Year, agora.Month, 5);
            var fimPeriodo = inicioPeriodo.AddMonths(1).AddDays(-1);
            var vencimento = fimPeriodo.AddDays(10); // por exemplo, 10 dias após o fechamento

            return new CreditCardInvoice
            {
                Id = Guid.NewGuid(),
                PeriodStart = inicioPeriodo,
                PeriodEnd = fimPeriodo,
                DueDate = vencimento,
                IsOpen = true
            };
        }
    }

    public class VirtualCreditCard
    {
        public Guid Id { get; set; }
        public string CreditCardNumber { get; set; }
        public string CreditCardName { get; set; }
        public string CreditCardCode { get; set; }
    }

    public class CreditCardInvoice
    {
        public Guid Id { get; set; }
        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }
        public DateTime DueDate { get; set; }
        public bool IsOpen { get; set; }

        public List<CreditCardExpense> Expenses { get; set; } = [];

        public decimal TotalAmount => Expenses.Sum(e => e.Amount);
    }

    public class CreditCardExpense
    {
        public Guid Id { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public ExpenseCategory Category { get; set; }
    }
}

using fusion.bank.core.Enum;

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
        public decimal CreditCardAvaliable { get => CreditCardLimit - CreditCardUsed; }
        public DateTime CreditCardValidity { get; set; }
        public CreditCardType CreditCardType { get; set; }
        public CreditCardFlag CreditCardFlag { get; set; }
        public bool CreditCardTried { get; set; }
        public int CreditCardTriedTimes { get; set; }
        public DateTime CreditCardNextAttempt { get; set; }
    }
}

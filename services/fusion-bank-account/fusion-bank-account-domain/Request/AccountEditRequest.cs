using fusion.bank.core.Enum;

namespace fusion.bank.account.domain.Request
{
    public class AccountEditRequest
    {
        public string Name { get; set; }
        public string LastName { get; set; }
        public decimal SalaryPerMonth { get; set; }
        public DocumentType DocumentType { get; set; }
        public string Email { get; set; }
        public string Document { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
        public string BirthDate { get; set; }
        public bool DarkMode { get; set; }
        public decimal ExpensePerDay { get; set; }
        public StatusAccount Status { get; set; }
    }
}

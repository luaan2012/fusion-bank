﻿using fusion.bank.core.Enum;

namespace fusion.bank.account.domain.Request
{
    public class AccountRequest
    {
        public string Name { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public decimal SalaryPerMonth { get; set; }
        public AccountType AccountType { get; set; }
        public string BankISBP { get; set; }
        public string BankName { get; set; }
        public DocumentType DocumentType { get; set; }
        public string Email { get; set; }
        public string Document { get; set; }
        public string Password { get; set; }
        public DateTime BirthDate { get; set; }
    }
}

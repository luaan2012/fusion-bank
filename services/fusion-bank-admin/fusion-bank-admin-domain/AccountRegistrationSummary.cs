namespace fusion.bank.admin.domain
{
    public class AccountRegistrationSummary
    {
        public int TotalLast24Hours { get; set; }
        public int[] MonthlyRegistrations { get; set; } // Array com 12 índices (jan-dez)
    }
}

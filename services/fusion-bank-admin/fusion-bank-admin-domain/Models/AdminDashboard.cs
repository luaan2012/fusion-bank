namespace fusion.bank.admin.domain.Models
{
    public class AdminDashboard
    {
        public double AmountTransfer { get; set; }
        public double AmountTed { get; set; }
        public double AmountPix { get; set; }
        public double AmountDoc { get; set; }
        public int RegisterCount { get; set; }
        public int[]? IncreaseMounthRegister { get; set; }
        public int TotalBilletsGenerate { get; set; }
        public double TotalBilletsAmount { get; set; }
        public int[]? BilletsSummary { get; set; }
        public int TotalDeposit { get; set; }
        public double TotalSystem { get; set; }
        public List<EventSummary> EventSummaries { get; set; }
    }
}

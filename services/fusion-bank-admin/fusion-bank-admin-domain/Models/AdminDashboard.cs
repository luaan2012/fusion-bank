namespace fusion.bank.admin.domain.Models
{
    public class AdminDashboard
    {
        public decimal AmountTransfer { get; set; }
        public decimal AmountTed { get; set; }
        public decimal AmountPix { get; set; }
        public decimal AmountDoc { get; set; }
        public decimal AmountRegister { get; set; }
        public int[]? IncreaseMounthRegister { get; set; }
        public int TotalBilletsGenerate { get; set; }
        public decimal TotalBilletsAmount { get; set; }
        public decimal TotalBilletsPayied { get; set; }
        public decimal TotalBilletsPayiedPercent { get; set; }
        public decimal TotalBilletsDue { get; set; }
        public decimal TotalBilletsExpire { get; set; }
        public decimal TotalDepositAmount { get; set; }
        public int TotalDeposit { get; set; }
        public decimal TotalSystem { get; set; }
        public List<SystemEvents> SystemEvents { get; set; }
    }

    public class SystemEvents
    {
        public DateTime DateTime { get; set; }
        public string Action { get; set; }
        public string User { get; set; }
        public string Amount { get; set; }
    }
}

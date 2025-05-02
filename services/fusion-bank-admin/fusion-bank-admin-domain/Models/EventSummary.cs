namespace fusion.bank.admin.domain.Models
{
    public class EventSummary
    {
        public DateTime EventDate { get; set; }
        public string Action { get; set; }
        public string User { get; set; }
        public double Amount { get; set; }
    }
}

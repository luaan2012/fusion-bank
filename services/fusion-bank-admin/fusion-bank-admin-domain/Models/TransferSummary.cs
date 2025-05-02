namespace fusion.bank.admin.domain.Models
{
    public class TransferSummary
    {
        public string Period { get; set; }
        public int TotalTransfers { get; set; }
        public int PixCount { get; set; }
        public int DocCount { get; set; }
        public int TedCount { get; set; }
    }
}

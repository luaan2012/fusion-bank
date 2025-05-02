namespace fusion.bank.admin.domain.Models
{
    public class BilletsSummary
    {
        public int TotalGenerated { get; set; }
        public double TotalAmount { get; set; }
        public int[] StatusCounts { get; set; } // [pago, vencido, a vencer]
    }
}

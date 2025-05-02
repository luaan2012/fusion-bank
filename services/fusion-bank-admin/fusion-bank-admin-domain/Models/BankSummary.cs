namespace fusion.bank.admin.domain.Models
{
    public class BankSummary
    {
        public int TotalAccounts { get; set; }
        public double TotalBalanceAllBanks { get; set; }
        public List<BankDetail> BankDetails { get; set; }
    }

    public class BankDetail
    {
        public string BankCode { get; set; }
        public int AccountCount { get; set; }
        public double TotalBalance { get; set; }
    }

    public class BankListItem
    {
        public string BankName { get; set; }
        public string IspbCode { get; set; }
        public int ActiveAccounts { get; set; }
    }

    public class BankAccountItem
    {
        public string Holder { get; set; }
        public string CpfCnpj { get; set; }
        public string AgencyAccount { get; set; }
        public double Balance { get; set; }
        public double Limit { get; set; }
        public string Status { get; set; }
    }

    public class BankUpdateModel
    {
        public string Name { get; set; }
        public int ActiveAccounts { get; set; }
    }
}

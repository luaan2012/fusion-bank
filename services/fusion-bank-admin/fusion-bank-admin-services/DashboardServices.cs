using fusion.bank.admin.domain.Interfaces;
using fusion.bank.admin.domain.Models;

namespace fusion.bank.admin.services
{
    public class DashboardServices(
        IAccountRepository accountRepository, IBankRepository bankRepository, ICreditCardRepository creditCardRepository,
        ITransferRepository transferRepository, IDepositRepository depositRepository, IInvestmentRepository investmentRepository, 
        IEventRepository eventRepository
        ) : IDashboardServices
    {
        public async Task<AdminDashboard> BuildAdminDashboard()
        {
            var billetsSummary = await depositRepository.GetBoletoSummaryAsync();
            var accountSummary = await accountRepository.GetAccountRegistrationSummaryAsync();
            var bankSummary = await bankRepository.GetBankSummaryAsync();
            var transferSummary = await transferRepository.GetTransferSummaryAsync("");
            var eventSummary = await eventRepository.GetRecentEventsAsync();

            var adminDashboard = new AdminDashboard()
            {
                TotalBilletsAmount = billetsSummary.TotalAmount,
                EventSummaries = eventSummary,
                BilletsSummary = billetsSummary.StatusCounts,
                AmountDoc = transferSummary.FirstOrDefault().DocCount,
                AmountPix = transferSummary.FirstOrDefault().PixCount,
                AmountTed = transferSummary.FirstOrDefault().TedCount,
                IncreaseMounthRegister = accountSummary.MonthlyRegistrations,
                RegisterCount = accountSummary.MonthlyRegistrations.Sum(),
                AmountTransfer = transferSummary.FirstOrDefault().TotalTransfers,
                TotalBilletsGenerate = billetsSummary.TotalGenerated,
                TotalSystem = bankSummary.TotalBalanceAllBanks
            };

            return adminDashboard;
        }
    }
}

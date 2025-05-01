using fusion.bank.admin.domain.Interfaces;
using fusion.bank.admin.domain.Models;

namespace fusion.bank.admin.services
{
    public class DashboardServices(
        IAccountRepository accountRepository, IBankRepository bankRepository, ICreditCardRepository creditCardRepository,
        ITransferRepository transferRepository, IDepositRepository depositRepository, IInvestmentRepository investmentRepository
        ) : IDashboardServices
    {
        public Task<AdminDashboard> BuildAdminDashboard()
        {

        }
    }
}

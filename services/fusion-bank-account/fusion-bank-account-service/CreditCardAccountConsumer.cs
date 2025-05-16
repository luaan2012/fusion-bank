using fusion.bank.account.domain.Interfaces;
using fusion.bank.core.Messages.DataContract;
using fusion.bank.core.Messages.Requests;
using fusion.bank.core.Messages.Responses;
using fusion.bank.core.Model.Errors;
using MassTransit;

namespace fusion.bank.account.service
{
    public class CreditCardAccountConsumer(IAccountRepository accountRepository) : IConsumer<NewAccountRequestInformation>
    {
        public async Task Consume(ConsumeContext<NewAccountRequestInformation> context)
        {
            var account = await accountRepository.ListAccountById(context.Message.id);

            if (account is null)
            {
                await context.RespondAsync(new DataContractMessage<AccountInformationResponse>().HandleError(new InexistentAccountError()));
                return;
            }

            var CreditCardRequestResponse = new AccountInformationResponse
            {
                AccountNumber = account.AccountNumber,
                AccountType = account.AccountType,
                KeyAccount = account.KeyAccount,
                Balance = account.Balance,
                BankISBP = account.BankISBP,
                BankName = account.BankName,
                FullName = account.FullName,
                LastName = account.LastName,
                Name = $"{account.Name} {account.LastName}",
                SalaryPerMonth = account.SalaryPerMonth,
                AverageBudgetPerMonth = new Random().Next(1600, 20000)
            };

            await context.RespondAsync(new DataContractMessage<AccountInformationResponse>().HandleSuccess(CreditCardRequestResponse));
        }
    }
}

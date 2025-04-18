using fusion.bank.account.domain.Interfaces;
using fusion.bank.core.Messages.DataContract;
using fusion.bank.core.Messages.Requests;
using fusion.bank.core.Messages.Responses;
using fusion.bank.core.Model.Errors;
using MassTransit;

namespace fusion.bank.account.service
{
    public class NewInvestmentRequestConsumer(IAccountRepository accountRepository) : IConsumer<NewInvestmentRequest>
    {
        public async Task Consume(ConsumeContext<NewInvestmentRequest> context)
        {
            var account = await accountRepository.ListAccountById(context.Message.AccountId);

            if(account == null)
            {
                await context.RespondAsync(new DataContractMessage<AccountInvestmentResponse>().HandleError(new InexistentAccountError()));

                return;
            }

            if(account.Balance < context.Message.Amount)
            {
                await context.RespondAsync(new DataContractMessage<AccountInvestmentResponse>().HandleError(new BalanceIsNotSufficient()));
            }

            account.Debit(context.Message.Amount);

            await accountRepository.UpdateAccount(account);

            await context.RespondAsync(new DataContractMessage<AccountInvestmentResponse>().HandleSuccess(new AccountInvestmentResponse()));
        }
    }
}

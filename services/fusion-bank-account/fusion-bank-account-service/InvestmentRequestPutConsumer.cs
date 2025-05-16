using fusion.bank.account.domain.Interfaces;
using fusion.bank.core.Messages.DataContract;
using fusion.bank.core.Messages.Requests;
using fusion.bank.core.Messages.Responses;
using fusion.bank.core.Model.Errors;
using MassTransit;

namespace fusion.bank.account.service
{
    public class InvestmentRequestPutConsumer(IAccountRepository accountRepository) : IConsumer<NewAccountRequestPutAmount>
    {
        public async Task Consume(ConsumeContext<NewAccountRequestPutAmount> context)
        {
            var account = await accountRepository.ListAccountById(context.Message.AccountId);

            if(account == null)
            {
                await context.RespondAsync(new DataContractMessage<AccountInvestmentResponse>().HandleError(new InexistentAccountError()));
                return;
            }

            account.Credit(context.Message.Amount);

            await accountRepository.UpdateAccount(account);

            await context.RespondAsync(new DataContractMessage<AccountInvestmentResponse>().HandleSuccess(new AccountInvestmentResponse()));
        }
    }
}

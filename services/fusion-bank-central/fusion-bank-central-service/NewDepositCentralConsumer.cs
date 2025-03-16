using fusion.bank.central.domain.Interfaces;
using fusion.bank.core.Messages.DataContract;
using fusion.bank.core.Messages.Requests;
using fusion.bank.core.Messages.Responses;
using MassTransit;

namespace fusion.bank.central.service
{
    public class NewDepositCentralConsumer(IPublishEndpoint bus, IBankRepository bankRepository) : IConsumer<NewDepositCentralRequest>
    {
        public async Task Consume(ConsumeContext<NewDepositCentralRequest> context)
        {
            var bankAccount = await bankRepository.ListAccountBankById(context.Message.AccountId);

            if (bankAccount == null)
            {
                return;
            }

            //await Task.Delay(8000);

            var accountUpdate = bankAccount.Accounts.FirstOrDefault(d => d.AccountId == context.Message.AccountId);

            if(accountUpdate is null)
            {
                return;
            }

            accountUpdate = accountUpdate with { Balance =  context.Message.Amount };

            bankAccount.UpdateAccount(context.Message.AccountId, accountUpdate);

            await bankRepository.UpdateBank(bankAccount);

            await context.RespondAsync(new DataContractMessage<DepositedCentralResponse>
            {
                Data = new DepositedCentralResponse(context.Message.DepositId, true),
                Success = true
            });
        }
    }
}

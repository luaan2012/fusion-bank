using fusion.bank.central.domain.Interfaces;
using fusion.bank.core.Messages.Requests;
using fusion.bank.core.Messages.Responses;
using fusion.bank.transfer.domain.Enum;
using MassTransit;

namespace fusion.bank.central.service
{
    public class NewKeyAccountCentralConsumer(IPublishEndpoint bus, IBankRepository bankRepository) : IConsumer<NewKeyAccountRequest>
    {
        public async Task Consume(ConsumeContext<NewKeyAccountRequest> context)
        {
            var bankAccount = await bankRepository.ListAccountBankById(context.Message.IdAccount);

            if (bankAccount == null)
            {
                return;
            }

            //await Task.Delay(8000);

            var accountUpdate = bankAccount.Accounts.FirstOrDefault(d => d.AccountId == context.Message.IdAccount);

            if (accountUpdate is null)
            {
                return;
            }

            accountUpdate = accountUpdate with { keyAccount = context.Message.KeyAccount };

            bankAccount.UpdateAccount(accountUpdate.AccountId, accountUpdate);

            await bankRepository.UpdateBank(bankAccount);

            await context.RespondAsync(new CreatedKeyAccountResponse(true));
        }
    }
}

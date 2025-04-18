using fusion.bank.account.domain.Interfaces;
using fusion.bank.core.Enum;
using fusion.bank.core.Messages.Producers;
using MassTransit;

namespace fusion.bank.account.service
{
    public class CreatedAccountConsumer(IAccountRepository accountRepository) : IConsumer<CreatedAccountProducer>
    {
        public async Task Consume(ConsumeContext<CreatedAccountProducer> context)
        {
            var account = await accountRepository.ListAccountById(context.Message.AccountId);

            if(account == null)
            {
                return;
            }

            account.Status = StatusAccount.Active;

            await accountRepository.UpdateAccount(account);
        }
    }
}

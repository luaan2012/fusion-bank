using fusion.bank.account.domain.Interfaces;
using fusion.bank.core.Enum;
using fusion.bank.core.Messages.Consumers;
using MassTransit;

namespace fusion.bank.account.Service
{
    public class CreatedAccount(IAccountRepository accountRepository) : IConsumer<CreatedAccountConsumer>
    {
        public async Task Consume(ConsumeContext<CreatedAccountConsumer> context)
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

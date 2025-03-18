using fusion.bank.account.domain.Interfaces;
using fusion.bank.core.Messages.DataContract;
using fusion.bank.core.Messages.Requests;
using fusion.bank.core.Messages.Responses;
using fusion.bank.core.Model.Errors;
using MassTransit;

namespace fusion.bank.account.service
{
    public class NewCreditCardCreatedAccountConsumer(IAccountRepository accountRepository) : IConsumer<NewCreditCardCreatedRequest>
    {
        public async Task Consume(ConsumeContext<NewCreditCardCreatedRequest> context)
        {
            var account = await accountRepository.ListAccountById(context.Message.CreditCard.AccountId);

            if (account is null)
            {
                await context.RespondAsync(new DataContractMessage<string>().HandleError(new InexistentAccountError()));
                return;
            }

            account.CreditCards ??= [];
            account.CreditCards.RemoveAll(d => d.Id == context.Message.CreditCard.Id);
            account.CreditCards.Add(context.Message.CreditCard);

            await accountRepository.UpdateAccount(account);

            await context.RespondAsync(new DataContractMessage<string>().HandleSuccess("Sucesso"));
        }
    }
}

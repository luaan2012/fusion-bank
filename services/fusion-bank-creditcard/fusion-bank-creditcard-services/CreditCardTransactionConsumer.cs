using fusion.bank.core.Messages.DataContract;
using fusion.bank.core.Messages.Requests;
using fusion.bank.core.Messages.Responses;
using fusion.bank.core.Model;
using fusion.bank.creditcard.domain.Interfaces;
using MassTransit;

namespace fusion.bank.account.service
{
    public class CreditCardTransactionConsumer(ICreditCardRepository creditCardRepository) : IConsumer<CreditCardTransactionRequest>
    {
        public async Task Consume(ConsumeContext<CreditCardTransactionRequest> context)
        {
            var credit = await creditCardRepository.ListCreditCardByAccountId(context.Message.AccountId);

            if (credit is null)
            {
                return;
            }

            var creditCardExpense = new CreditCardExpense
            {
                Amount = context.Message.Amount,
                Category = context.Message.ExpenseCategory,
                Date = DateTime.UtcNow,
                Description = context.Message.Description,
                Id = Guid.NewGuid(),
            };

            credit.AdicionarGastoComFatura(creditCardExpense);

            await creditCardRepository.SaveTriedCard(credit);

            await context.RespondAsync(new DataContractMessage<CreditCardTransactionResponse>().HandleSuccess(new CreditCardTransactionResponse(true)));
        }
    }
}

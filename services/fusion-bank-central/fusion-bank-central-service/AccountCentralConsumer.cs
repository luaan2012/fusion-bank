using fusion.bank.central.domain.Interfaces;
using fusion.bank.core.Messages.DataContract;
using fusion.bank.core.Messages.Producers;
using MassTransit;

namespace fusion.bank.central.service
{
    public class AccountCentralConsumer(IBankRepository bankRepository) : IConsumer<NewAccountProducer>
    {
        public async Task Consume(ConsumeContext<NewAccountProducer> context)
        {
            var bankAccount = await bankRepository.ListBankByISPB(context.Message.BankISBP);

            if (bankAccount == null)
            {
                return;
            }

            bankAccount.AddAccount(context.Message);

            await bankRepository.UpdateBank(bankAccount);

            await context.RespondAsync(new DataContractMessage<CreatedAccountResponse> { Success = true});
        }
    }
}

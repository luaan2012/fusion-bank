using fusion.bank.central.domain.Interfaces;
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

            await Task.Delay(8000);

            bankAccount.AddAccount(context.Message);

            await bankRepository.UpdateBank(bankAccount);

            await context.RespondAsync(new CreatedAccountResponse(context.Message.AccountId, context.Message.Name, context.Message.LastName, context.Message.FullName, context.Message.AccountNumber,
                context.Message.Balance, context.Message.TransferLimit, context.Message.SalaryPerMonth, context.Message.AccountType, bankAccount.ISPB, bankAccount.Name));
        }
    }
}

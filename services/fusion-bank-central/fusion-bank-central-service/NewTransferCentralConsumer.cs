using fusion.bank.central.domain.Interfaces;
using fusion.bank.core.Messages.Requests;
using fusion.bank.core.Messages.Responses;
using fusion.bank.transfer.domain.Enum;
using MassTransit;

namespace fusion.bank.central.service
{
    public class NewTransferCentralConsumer(IPublishEndpoint bus, IBankRepository bankRepository) : IConsumer<NewTransferCentralRequest>
    {
        public async Task Consume(ConsumeContext<NewTransferCentralRequest> context)
        {
            var bankAccount = context.Message.TransferType switch
            {
                TransferType.PIX => (await bankRepository.ListAccountBankByKeyAccount(context.Message.KeyAccount)),
                TransferType.DOC => await bankRepository.ListAccountBankByAccountNumber(context.Message.KeyAccount),
                TransferType.TED => await bankRepository.ListAccountBankByAccountNumber(context.Message.KeyAccount),
            };

            if (bankAccount == null)
            {
                return;
            }

            //await Task.Delay(8000);

            var accountUpdate = context.Message.TransferType switch
            {
                TransferType.PIX => bankAccount.Accounts.FirstOrDefault(d => d.keyAccount == context.Message.KeyAccount),
                TransferType.DOC => bankAccount.Accounts.FirstOrDefault(d => d.AccountNumber == context.Message.KeyAccount),
                TransferType.TED => bankAccount.Accounts.FirstOrDefault(d => d.AccountNumber == context.Message.KeyAccount),
            };

            if (accountUpdate is null)
            {
                return;
            }

            accountUpdate = accountUpdate with { Balance = context.Message.Amount };

            bankAccount.UpdateAccount(accountUpdate.AccountId, accountUpdate);

            await bankRepository.UpdateBank(bankAccount);

            await context.RespondAsync(new TransferredCentralResponse(true));
        }
    }
}

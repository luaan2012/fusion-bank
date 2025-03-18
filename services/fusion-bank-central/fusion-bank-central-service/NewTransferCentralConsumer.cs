using fusion.bank.central.domain.Interfaces;
using fusion.bank.core.Messages.DataContract;
using fusion.bank.core.Messages.Requests;
using fusion.bank.core.Messages.Responses;
using fusion.bank.core.Enum;
using MassTransit;

namespace fusion.bank.central.service
{
    public class NewTransferCentralConsumer(IPublishEndpoint bus, IBankRepository bankRepository) : IConsumer<NewTransferCentralRequest>
    {
        public async Task Consume(ConsumeContext<NewTransferCentralRequest> context)
        {
            var bankAccountReceive = context.Message.TransferType switch
            {
                TransferType.PIX => (await bankRepository.ListAccountBankByKeyAccount(context.Message.KeyAccount)),
                TransferType.DOC => await bankRepository.ListAccountBankByAccountNumber(context.Message.KeyAccount),
                TransferType.TED => await bankRepository.ListAccountBankByAccountNumber(context.Message.KeyAccount),
            };

            var bankAccountPayer = await bankRepository.ListAccountBankByAccountNumber(context.Message.AccountOwner);

            if (bankAccountReceive is null && bankAccountPayer is null)
            {
                return;
            }

            //await Task.Delay(8000);

            var accountUpdateReceive = context.Message.TransferType switch
            {
                TransferType.PIX => bankAccountReceive.Accounts.FirstOrDefault(d => d.keyAccount == context.Message.KeyAccount),
                TransferType.DOC => bankAccountReceive.Accounts.FirstOrDefault(d => d.AccountNumber == context.Message.KeyAccount),
                TransferType.TED => bankAccountReceive.Accounts.FirstOrDefault(d => d.AccountNumber == context.Message.KeyAccount),
            };

            var accountUpdatePayer = bankAccountReceive.Accounts.FirstOrDefault(d => d.keyAccount == context.Message.KeyAccount);

            if (accountUpdateReceive is null || accountUpdatePayer is null)
            {
                return;
            }

            accountUpdateReceive = accountUpdateReceive with { Balance = context.Message.Amount };

            accountUpdatePayer = accountUpdatePayer with { Balance = context.Message.Amount };

            bankAccountReceive.UpdateAccount(accountUpdateReceive.AccountId, accountUpdateReceive);

            bankAccountPayer.UpdateAccount(accountUpdatePayer.AccountId, accountUpdatePayer);

            await bankRepository.UpdateBank(bankAccountReceive);

            await bankRepository.UpdateBank(bankAccountPayer);

            await context.RespondAsync(new DataContractMessage<TransferredCentralResponse>
            {
                Data = new TransferredCentralResponse(true),
                Success = true
            });
        }
    }
}

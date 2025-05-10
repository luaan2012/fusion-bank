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
            var bankAccountReceiver = context.Message.TransferType switch
            {
                TransferType.PIX => await bankRepository.ListAccountBankByKeyAccount(context.Message.KeyAccount),
                TransferType.DOC or TransferType.TED => await bankRepository.ListAccountBankByAccountAgencyNumber(context.Message.AccountReceiver, context.Message.AgencyReceiver)
            };

            var bankAccountPayer = await bankRepository.ListAccountBankByAccountNumber(context.Message.AccountPayer);

            if (bankAccountReceiver is null && bankAccountPayer is null)
            {
                return;
            }

            //await Task.Delay(8000);

            var accountUpdateReceiver = context.Message.TransferType switch
            {
                TransferType.PIX => bankAccountReceiver.Accounts.FirstOrDefault(d => d.KeyAccount == context.Message.KeyAccount),
                TransferType.DOC => bankAccountReceiver.Accounts.FirstOrDefault(d => d.AccountNumber == context.Message.AccountReceiver && d.Agency == context.Message.AgencyReceiver),
                TransferType.TED => bankAccountReceiver.Accounts.FirstOrDefault(d => d.AccountNumber == context.Message.AccountReceiver && d.Agency == context.Message.AgencyReceiver),
            };

            var accountUpdatePayer = bankAccountReceiver.Accounts.FirstOrDefault(d => d.AccountId == context.Message.AccountId);

            if (accountUpdateReceiver is null || accountUpdatePayer is null)
            {
                return;
            }

            accountUpdateReceiver = accountUpdateReceiver with { Balance = context.Message.AmountReceiver };

            accountUpdatePayer = accountUpdatePayer with { Balance = context.Message.AmountPayer };

            bankAccountReceiver.UpdateAccount(accountUpdateReceiver.AccountId, accountUpdateReceiver);

            bankAccountPayer.UpdateAccount(accountUpdatePayer.AccountId, accountUpdatePayer);

            await bankRepository.UpdateBank(bankAccountReceiver);

            await bankRepository.UpdateBank(bankAccountPayer);

            await context.RespondAsync(new DataContractMessage<TransferredCentralResponse>
            {
                Data = new TransferredCentralResponse(true),
                Success = true
            });
        }
    }
}

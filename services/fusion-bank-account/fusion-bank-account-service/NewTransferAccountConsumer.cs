using fusion.bank.account.domain.Interfaces;
using fusion.bank.core.Messages.DataContract;
using fusion.bank.core.Messages.Requests;
using fusion.bank.core.Messages.Responses;
using fusion.bank.core.Model.Errors;
using fusion.bank.transfer.domain.Enum;
using MassTransit;

namespace fusion.bank.account.service
{
    public class NewTransferAccountConsumer(IAccountRepository accountRepository, IRequestClient<NewTransferCentralRequest> requestClient) : IConsumer<NewTransferAccountRequest>
    {
        public async Task Consume(ConsumeContext<NewTransferAccountRequest> context)
        {
            var accountReceive = context.Message.TransferType switch
            {
                TransferType.PIX => await accountRepository.ListAccountByKey(context.Message.KeyAccount),
                TransferType.TED => await accountRepository.ListAccountByNumberAccount(context.Message.KeyAccount),
                TransferType.DOC => await accountRepository.ListAccountByNumberAccount(context.Message.KeyAccount),
            };

            var accountOwner = await accountRepository.ListAccountByNumberAccount(context.Message.AccountOwner);

            if (accountReceive is null || accountOwner is null)
            {
                await context.RespondAsync(accountReceive is null
                    ? new DataContractMessage<TransferredAccountResponse>().HandleError(new InexistentAccountReceiveError())
                    : new DataContractMessage<TransferredAccountResponse>().HandleError(new InexistentAccountPayedError()));

                return;
            }

            accountOwner.Debit(context.Message.Amount);

            //await Task.Delay(new Random().Next(5, 30) * 1000);

            accountReceive.Credit(context.Message.Amount);

            var response = await requestClient.GetResponse<DataContractMessage<TransferredCentralResponse>>(new NewTransferCentralRequest(context.Message.TransferType, accountReceive.Balance, context.Message.KeyAccount, context.Message.AccountOwner));

            if (!response.Message.Success)
            {
                return;
            }

            await accountRepository.UpdateAccount(accountReceive);

            await context.RespondAsync(new DataContractMessage<TransferredAccountResponse>().HandleSuccess(new TransferredAccountResponse(true)));
        }
    }
}

using fusion.bank.account.domain.Interfaces;
using fusion.bank.core.Messages.DataContract;
using fusion.bank.core.Messages.Requests;
using fusion.bank.core.Messages.Responses;
using fusion.bank.core.Model.Errors;
using fusion.bank.core.Enum;
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

            accountReceive.Credit(context.Message.Amount);

            var response = await requestClient.GetResponse<DataContractMessage<TransferredCentralResponse>>(new NewTransferCentralRequest(context.Message.TransferType, accountOwner.AccountId, accountReceive.Balance, accountOwner.Balance, context.Message.KeyAccount, context.Message.AccountOwner));

            if (!response.Message.Success)
            {
                return;
            }

            await accountRepository.UpdateAccount(accountReceive);

            await accountRepository.UpdateAccount(accountOwner);

            await context.RespondAsync(new DataContractMessage<TransferredAccountResponse>().HandleSuccess(new TransferredAccountResponse(true)));
        }
    }
}

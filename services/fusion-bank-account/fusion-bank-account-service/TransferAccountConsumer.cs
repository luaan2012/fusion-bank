using fusion.bank.account.domain.Interfaces;
using fusion.bank.core.Messages.DataContract;
using fusion.bank.core.Messages.Requests;
using fusion.bank.core.Messages.Responses;
using fusion.bank.core.Model.Errors;
using fusion.bank.core.Enum;
using MassTransit;

namespace fusion.bank.account.service
{
    public class TransferAccountConsumer(IAccountRepository accountRepository, IRequestClient<TransactionCentralRequest> requestClient) : IConsumer<NewTransferAccountRequest>
    {
        public async Task Consume(ConsumeContext<NewTransferAccountRequest> context)
        {
            var accountReceiver = context.Message.TransferType switch
            {
                TransferType.PIX => await accountRepository.ListAccountByKey(context.Message.KeyAccount),
                TransferType.TED or TransferType.DOC => await accountRepository.ListAccountByNumberAgencyAccount(context.Message.AccountReceiver, context.Message.AgencyReceiver),
            };

            var AccountPayer = await accountRepository.ListAccountByNumberAccount(context.Message.AccountPayer);

            if (accountReceiver is null || AccountPayer is null)
            {
                await context.RespondAsync(accountReceiver is null
                    ? new DataContractMessage<TransferredAccountResponse>().HandleError(new InexistentAccountReceiveError())
                    : new DataContractMessage<TransferredAccountResponse>().HandleError(new InexistentAccountPayedError()));

                return;
            }

            AccountPayer.Debit(context.Message.Amount);

            accountReceiver.Credit(context.Message.Amount);

            var response = await requestClient.GetResponse<DataContractMessage<TransferredCentralResponse>>(new TransactionCentralRequest(AccountPayer.AccountId, accountReceiver.Balance, 
                AccountPayer.Balance, AccountPayer.AccountNumber, AccountPayer.Agency, context.Message.AccountReceiver, context.Message.AgencyReceiver, true, true));

            if (!response.Message.Success)
            {
                return;
            }

            await accountRepository.UpdateAccount(accountReceiver);

            await accountRepository.UpdateAccount(AccountPayer);

            await context.RespondAsync(new DataContractMessage<TransferredAccountResponse>().HandleSuccess(new TransferredAccountResponse(true, accountReceiver.AccountId, accountReceiver.FullName)));
        }
    }
}

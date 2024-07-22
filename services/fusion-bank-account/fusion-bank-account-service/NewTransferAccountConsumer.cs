using fusion.bank.account.domain.Interfaces;
using fusion.bank.core.Messages.Requests;
using fusion.bank.core.Messages.Responses;
using fusion.bank.transfer.domain.Enum;
using MassTransit;

namespace fusion.bank.account.service
{
    public class NewTransferAccountConsumer(IAccountRepository accountRepository, IPublishEndpoint bus, IRequestClient<NewTransferCentralRequest> requestClient) : IConsumer<NewTransferAccountRequest>
    {
        public async Task Consume(ConsumeContext<NewTransferAccountRequest> context)
        {
            var account = context.Message.TransferType switch
            {
                TransferType.PIX => await accountRepository.ListAccountByKey(context.Message.KeyAccount),
                TransferType.TED => await accountRepository.ListAccountByNumberAccount(context.Message.KeyAccount),
                TransferType.DOC => await accountRepository.ListAccountByNumberAccount(context.Message.KeyAccount),
            };

            if(account is null)
            {
                return;
            }

            //await Task.Delay(new Random().Next(5, 30) * 1000);

            account.Credit(context.Message.Amount);

            var response = await requestClient.GetResponse<TransferredCentralResponse>(new NewTransferCentralRequest(context.Message.TransferType, account.Balance, context.Message.KeyAccount));

            if (!response.Message.Transferred)
            {
                return;
            }

            await accountRepository.UpdateAccount(account);

            await context.RespondAsync<TransferredAccountResponse>(new TransferredAccountResponse(true));
        }
    }
}

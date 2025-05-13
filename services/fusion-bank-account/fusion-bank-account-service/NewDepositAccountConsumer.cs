using fusion.bank.account.domain.Interfaces;
using fusion.bank.core.Messages.DataContract;
using fusion.bank.core.Messages.Producers;
using fusion.bank.core.Messages.Requests;
using fusion.bank.core.Messages.Responses;
using MassTransit;

namespace fusion.bank.account.service
{
    public class NewDepositAccountConsumer(IAccountRepository accountRepository, IPublishEndpoint bus, IRequestClient<NewDepositCentralRequest> requestClient) : IConsumer<NewDepositAccountProducer>
    {
        public async Task Consume(ConsumeContext<NewDepositAccountProducer> context)
        {
            var account = await accountRepository.ListAccountById(context.Message.AccountId);

            if(account is null)
            {
                return;
            }

            account.Credit(context.Message.Amount);

            var response = await requestClient.GetResponse<DataContractMessage<DepositedCentralResponse>>(
                new NewDepositCentralRequest(context.Message.AccountId, context.Message.DepositId, context.Message.AccountNumberReceiver, 
                context.Message.AgencyNumberReceiver, account.Balance));

            if (!response.Message.Success)
            {
                return;
            }

            await accountRepository.UpdateAccount(account);

            await bus.Publish(new DepositedAccountProducer(context.Message.DepositId, true));
        }
    }
}

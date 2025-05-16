using fusion.bank.account.domain.Interfaces;
using fusion.bank.core.Messages.DataContract;
using fusion.bank.core.Messages.Producers;
using fusion.bank.core.Messages.Requests;
using fusion.bank.core.Messages.Responses;
using MassTransit;

namespace fusion.bank.account.service
{
    public class DepositAccountConsumer(IAccountRepository accountRepository, IPublishEndpoint bus, IRequestClient<TransactionCentralRequest> requestClient) : IConsumer<NewDepositAccountProducer>
    {
        public async Task Consume(ConsumeContext<NewDepositAccountProducer> context)
        {
            var account = await accountRepository.ListAccountById(context.Message.AccountId);

            if(account is null)
            {
                return;
            }

            account.Credit(context.Message.Amount);
            account.AddExpense(DateTime.Now, context.Message.Amount, context.Message.ExpenseCategory);

            var response = await requestClient.GetResponse<DataContractMessage<TransferredCentralResponse>>(new TransactionCentralRequest(account.AccountId, 
                account.Balance, AccountReceiver: account.AccountNumber, AgencyReceiver: account.Agency, UpdateReceiver: true));


            if (!response.Message.Success)
            {
                return;
            }

            await accountRepository.UpdateAccount(account);

            await bus.Publish(new DepositedAccountProducer(context.Message.DepositId, context.Message.AccountId, null, true));
        }
    }
}

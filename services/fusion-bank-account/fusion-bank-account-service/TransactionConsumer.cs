using fusion.bank.account.domain.Interfaces;
using fusion.bank.core.Enum;
using fusion.bank.core.Messages.DataContract;
using fusion.bank.core.Messages.Producers;
using fusion.bank.core.Messages.Requests;
using fusion.bank.core.Messages.Responses;
using fusion.bank.core.Model.Errors;
using fusion.bank.deposit.domain;
using MassTransit;

namespace fusion.bank.account.service
{
    public class TransactionConsumer(IAccountRepository accountRepository, 
        IPublishEndpoint bus,
        IRequestClient<TransactionCentralRequest> transactionRequestClient, 
        IRequestClient<CreditCardTransactionRequest> cardRequestClient) : IConsumer<TransactionRequest>
    {
        public async Task Consume(ConsumeContext<TransactionRequest> context)
        {
            var isDeposit = context.Message.ExpenseCategory == ExpenseCategory.DEPOSIT;
            var paymentTypeDebit = context.Message.PaymentType == PaymentType.DEBIT;

            var accountPayer = await accountRepository.ListAccountById(context.Message.AccountId);
            var accountReceiver = isDeposit ? await accountRepository.ListAccountByNumberAgencyAccount(context.Message.AccountReceiver, context.Message.AgencyReceiver) : null;

            if(accountPayer == null || (isDeposit && accountReceiver is null))
            {
                await context.RespondAsync(accountReceiver is null
                    ? new DataContractMessage<TransferredAccountResponse>().HandleError(new InexistentAccountReceiveError())
                    : new DataContractMessage<TransferredAccountResponse>().HandleError(new InexistentAccountPayedError()));

                return;
            }

            if (isDeposit && paymentTypeDebit)
            {
                var responseCentral = await transactionRequestClient.GetResponse<DataContractMessage<TransferredCentralResponse>>(new TransactionCentralRequest(accountPayer.AccountId, accountReceiver.Balance,
                    accountPayer.Balance, accountPayer.AccountNumber, accountPayer.Agency, context.Message.AccountReceiver, context.Message.AgencyReceiver, true, true));

                if (!responseCentral.Message.Success)
                {
                    return;
                }

                accountPayer.Debit(context.Message.Amount);
                accountReceiver.Credit(context.Message.Amount);
            }

            if (!isDeposit && paymentTypeDebit)
            {
                accountPayer.Debit(context.Message.Amount);
            }

            if (!isDeposit && !paymentTypeDebit)
            {
                var responseCard = await cardRequestClient.GetResponse<DataContractMessage<CreditCardTransactionResponse>>(new CreditCardTransactionRequest(
                    accountPayer.AccountId, context.Message.Amount, context.Message.ExpenseCategory, context.Message.Description));

                if (!responseCard.Message.Success)
                {
                    return;
                }
            }

            if (isDeposit && !paymentTypeDebit)
            {
                var responseCard = await cardRequestClient.GetResponse<DataContractMessage<CreditCardTransactionResponse>>(new CreditCardTransactionRequest(
                    accountPayer.AccountId, context.Message.Amount, context.Message.ExpenseCategory, context.Message.Description));

                var responseCentral = await transactionRequestClient.GetResponse<DataContractMessage<TransferredCentralResponse>>(new TransactionCentralRequest(accountPayer.AccountId, accountReceiver.Balance,
                    accountPayer.Balance, accountPayer.AccountNumber, accountPayer.Agency, context.Message.AccountReceiver, context.Message.AgencyReceiver, true, true));

                if (!responseCard.Message.Success && !responseCentral.Message.Success)
                {
                    return;
                }

                accountReceiver.Credit(context.Message.Amount);
            }

            if(isDeposit)
            {
                await accountRepository.UpdateAccount(accountReceiver);
            }

            accountPayer.AddExpense(DateTime.Now, context.Message.Amount, context.Message.ExpenseCategory);
            await accountRepository.UpdateAccount(accountPayer);

            await bus.Publish(new DepositedAccountProducer(context.Message.DepositId, accountPayer.AccountId, accountReceiver?.AccountId, true));
        }
    }
}

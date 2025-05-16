using fusion.bank.central.domain.Interfaces;
using fusion.bank.core.Messages.DataContract;
using fusion.bank.core.Messages.Requests;
using fusion.bank.core.Messages.Responses;
using MassTransit;

namespace fusion.bank.central.service
{
    public class TransactionCentralConsumer(IPublishEndpoint bus, IBankRepository bankRepository) : IConsumer<TransactionCentralRequest>
    {
        public async Task Consume(ConsumeContext<TransactionCentralRequest> context)
        {
            var bankAccountReceiver = context.Message.UpdateReceiver
            ? await bankRepository.ListAccountBankByAccountAgencyNumber(context.Message.AccountReceiver, context.Message.AgencyReceiver)
            : null;

            var bankAccountPayer = context.Message.UpdatePayer
                ? await bankRepository.ListAccountBankByAccountAgencyNumber(context.Message.AccountPayer, context.Message.AgencyPayer)
                : null;

            if (bankAccountReceiver is null && bankAccountPayer is null)
            {
                return;
            }

            if (context.Message.UpdateReceiver)
            {
                var accountUpdateReceiver = bankAccountReceiver?.Accounts
                    .FirstOrDefault(d => d.AccountNumber == context.Message.AccountReceiver && d.Agency == context.Message.AgencyReceiver);

                if (accountUpdateReceiver is not null)
                {
                    accountUpdateReceiver = accountUpdateReceiver with { Balance = context.Message.BalanceReceiver };
                    bankAccountReceiver!.UpdateAccount(accountUpdateReceiver.AccountId, accountUpdateReceiver);
                    await bankRepository.UpdateBank(bankAccountReceiver);
                }
            }

            if (context.Message.UpdatePayer)
            {
                var accountUpdatePayer = bankAccountPayer?.Accounts
                    .FirstOrDefault(d => d.AccountId == context.Message.AccountId);

                if (accountUpdatePayer is not null)
                {
                    accountUpdatePayer = accountUpdatePayer with { Balance = context.Message.BalancePayer };
                    bankAccountPayer!.UpdateAccount(accountUpdatePayer.AccountId, accountUpdatePayer);
                    await bankRepository.UpdateBank(bankAccountPayer);
                }
            }

            await context.RespondAsync(new DataContractMessage<TransferredCentralResponse>
            {
                Data = new TransferredCentralResponse(true),
                Success = true
            });
        }
    }
}

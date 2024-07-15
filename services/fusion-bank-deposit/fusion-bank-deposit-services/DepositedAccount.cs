using fusion.bank.core.Messages.Consumers;
using fusion.bank.deposit.domain.Interfaces;
using MassTransit;

namespace fusion_bank_deposit_services
{
    public class DepositedAccount(IDepositRepository depositRepository) : IConsumer<DepositedAccountConsumer>
    {
        public async Task Consume(ConsumeContext<DepositedAccountConsumer> context)
        {
            var deposit = await depositRepository.GetDepositById(context.Message.DepositId);

            if(deposit is null)
            {
                return;
            }

            deposit.ConsuptionDate();
            deposit.DebitedTrue();
            deposit.DisableDeposit();

            await depositRepository.UpdateDeposit(deposit);
        }
    }
}

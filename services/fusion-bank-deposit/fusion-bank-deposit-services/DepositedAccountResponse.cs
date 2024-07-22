using fusion.bank.core.Messages.Producers;
using fusion.bank.deposit.domain.Interfaces;
using MassTransit;

namespace fusion.bank.deposit.services
{
    public class DepositedAccountResponse(IDepositRepository depositRepository) : IConsumer<DepositedAccountProducer>
    {
        public async Task Consume(ConsumeContext<DepositedAccountProducer> context)
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

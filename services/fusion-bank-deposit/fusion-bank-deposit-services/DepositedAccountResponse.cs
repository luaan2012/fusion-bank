﻿using fusion.bank.core.Enum;
using fusion.bank.core;
using fusion.bank.core.Messages.Producers;
using fusion.bank.deposit.domain.Interfaces;
using MassTransit;
using fusion.bank.deposit.domain;

namespace fusion.bank.deposit.services
{
    public class DepositedAccountResponse(IDepositRepository depositRepository, IPublishEndpoint bus) : IConsumer<DepositedAccountProducer>
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

            if(deposit.BilletType == ExpenseCategory.DEPOSIT && deposit.DirectToAccount)
            {
                var eventDeposit = GenerateEvent.CreateDirectDepositMadeEvent(deposit.AccountId.ToString(), deposit.Amount, TransferType.BOLETO);
                await bus.Publish(eventDeposit);
            }
        }
    }
}

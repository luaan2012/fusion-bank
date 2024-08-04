using fusion.bank.core.Messages.Requests;
using fusion.bank.core.Messages.Responses;
using fusion.bank.transfer.domain.Interfaces;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace fusion.bank.transfer.services
{
    public class TransferBackground(ILogger<TransferBackground> _logger, IServiceProvider _serviceProvider) : IHostedService, IDisposable
    {
        //private readonly ILogger<TransferBackground> _logger;   
        private Timer _timer;

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Timed Background Service is starting.");

            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(60));

            return Task.CompletedTask;
        }

        private async void DoWork(object state)
        {
            _logger.LogInformation("Executing DoWork at: {time}", DateTimeOffset.Now);

            using(var create = _serviceProvider.CreateScope())
            {
                var transferRepository = create.ServiceProvider.GetRequiredService<ITransferRepository>();
                var requestClient = create.ServiceProvider.GetRequiredService<IRequestClient<NewTransferAccountRequest>>();

                var schedules = await transferRepository.ListAllSchedules();

                foreach (var schedule in schedules)
                {
                    var response = await requestClient.GetResponse<TransferredAccountResponse>(new NewTransferAccountRequest(schedule.TransferType, schedule.KeyAccount, schedule.Amount, schedule.AccountNumberOwner));

                    if (response.Message.Transferred)
                    {
                        schedule.TransferStatus = domain.Enum.TransferStatus.COMPLETE;
                        await transferRepository.UpdateTransfer(schedule);
                    }
                }
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Timed Background Service is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}

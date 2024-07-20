using fusion.bank.transfer.domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace fusion.bank.transfer.services
{
    public class TransferBackground : IHostedService, IDisposable
    {
        private readonly ILogger<TransferBackground> _logger;   
        private readonly IServiceProvider _serviceProvider;
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

                var schedules = await transferRepository.ListAllSchedules();

                foreach (var schedule in schedules)
                {

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

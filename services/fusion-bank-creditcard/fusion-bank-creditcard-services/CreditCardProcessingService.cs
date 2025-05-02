using fusion.bank.creditcard.services.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace fusion.bank.creditcard.services
{
    public class CreditCardProcessingService : BackgroundService
    {
        private readonly IBackgroundTaskQueue _taskQueue;
        private readonly ILogger<CreditCardProcessingService> _logger;

        public CreditCardProcessingService(IBackgroundTaskQueue taskQueue, ILogger<CreditCardProcessingService> logger)
        {
            _taskQueue = taskQueue;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("CreditCardProcessingService iniciado.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var workItem = await _taskQueue.DequeueAsync(stoppingToken);
                    await workItem(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao processar tarefa em segundo plano.");
                }
            }

            _logger.LogInformation("CreditCardProcessingService encerrado.");
        }
    }
}

using fusion.bank.core.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace fusion.bank.transfer.services
{
    public class TransferProcessingService : BackgroundService
    {
        private readonly IBackgroundTaskQueue _taskQueue;
        private readonly ILogger<TransferProcessingService> _logger;

        public TransferProcessingService(IBackgroundTaskQueue taskQueue, ILogger<TransferProcessingService> logger)
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

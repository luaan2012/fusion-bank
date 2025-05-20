using fusion.bank.core.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace fusion.bank.core.Services
{
    public class ProcessingBackGroundService : BackgroundService
    {
        private readonly IBackgroundTaskQueue _taskQueue;
        private readonly ILogger<ProcessingBackGroundService> _logger;

        public ProcessingBackGroundService(IBackgroundTaskQueue taskQueue, ILogger<ProcessingBackGroundService> logger)
        {
            _taskQueue = taskQueue;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("ProcessingBackGroundService iniciado.");

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

            _logger.LogInformation("ProcessingBackGroundService encerrado.");
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Helix.BackgroundWorker.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Helix.BackgroundWorker.Services
{
    class WorkQueueHostedService : BackgroundService
    {
        private readonly ILogger<WorkQueueHostedService> _logger;
        private readonly IWorkQueueService _workQueueService;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public WorkQueueHostedService(ILogger<WorkQueueHostedService> logger, IWorkQueueService workQueueService, IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger;
            _workQueueService = workQueueService;
            _serviceScopeFactory = serviceScopeFactory;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Starting background queue service");
            await StartProcessing(stoppingToken);
        }

        private async ValueTask StartProcessing(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var task = await _workQueueService.DequeueAsync(stoppingToken);

                try
                {
                    await task(_serviceScopeFactory, stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occured during execution of a background task {task}", task);
                }
            }
        }
    }
}

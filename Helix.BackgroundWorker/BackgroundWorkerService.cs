using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Helix.BackgroundWorker.Absractions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Helix.BackgroundWorker
{
    //https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/hosted-services?view=aspnetcore-5.0&tabs=visual-studio
    class BackgroundWorkerService : BackgroundService
    {
        private readonly ILogger<BackgroundWorkerService> _logger;
        private readonly IBackgroundTaskQueue _backgroundTaskQueue;

        public BackgroundWorkerService(ILogger<BackgroundWorkerService> logger, IBackgroundTaskQueue backgroundTaskQueue, IServiceProvider services)
        {
            _logger = logger;
            _backgroundTaskQueue = backgroundTaskQueue;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Background Task queue is running");
            await ProcessItems(stoppingToken);
        }

        private async ValueTask ProcessItems(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var workEvent = await _backgroundTaskQueue.DequeueAsync(cancellationToken);
                try
                {
                    _logger.LogInformation("Executing {workEvent}", workEvent);
                }
                catch (Exception e)
                {
                    _logger.LogError("Error occurred executing {item}\n{ex}", workEvent, e);
                }
            }
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Queued Hosted Service is stopping.");

            await base.StopAsync(stoppingToken);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Helix.BackgroundWorker.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Helix.BackgroundWorker.Services
{
    class WorkQueueService : IWorkQueueService
    {
        private readonly ILogger<WorkQueueService> _logger;
        private readonly Channel<Func<IServiceScopeFactory, CancellationToken, ValueTask>> _queue;

        public WorkQueueService(ILogger<WorkQueueService> logger)
        {
            _logger = logger;
            var options = new BoundedChannelOptions(1000)
            {
                FullMode = BoundedChannelFullMode.DropOldest
            };

            _queue = Channel.CreateBounded<Func<IServiceScopeFactory, CancellationToken, ValueTask>>(options);
        }

        public async ValueTask QueueAsync(Func<IServiceScopeFactory, CancellationToken, ValueTask> task)
        {
            if (task == null)
                throw new ArgumentNullException(nameof(task));

            await _queue.Writer.WriteAsync(task);
        }

        public async ValueTask QueueAsync<T>(Func<T, ValueTask> task)
        {
            await QueueAsync(async (serviceScopeFactory, cancellationToken) =>
            {
                using var scope = serviceScopeFactory.CreateScope();
                var myService = scope.ServiceProvider.GetRequiredService<T>();

                try
                {
                    _logger.LogDebug("Queuing {task}", task);
                    await task(myService);
                }
                catch (Exception ex)
                {
                    _logger.LogError("Error occured during {task}\n{ex}", task, ex);
                }
            });
        }

        public async ValueTask<Func<IServiceScopeFactory, CancellationToken, ValueTask>> DequeueAsync(CancellationToken cancellationToken)
        {
            var workItem = await _queue.Reader.ReadAsync(cancellationToken);

            return workItem;
        }
    }
}

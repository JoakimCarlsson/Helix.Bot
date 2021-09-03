using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Helix.BackgroundWorker.Abstractions
{
    public interface IWorkQueueService
    {
        ValueTask QueueAsync(Func<IServiceScopeFactory, CancellationToken, ValueTask> task);
        ValueTask QueueAsync<T>(Func<T, ValueTask> task);
        ValueTask<Func<IServiceScopeFactory, CancellationToken, ValueTask>> DequeueAsync(CancellationToken cancellationToken);
    }
}

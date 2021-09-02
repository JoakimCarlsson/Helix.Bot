using System;
using System.Threading;
using System.Threading.Tasks;

namespace Helix.BackgroundWorker.Absractions
{
    interface IBackgroundTaskQueue
    {
        ValueTask QueueAsync(Func<CancellationToken, ValueTask> workItem);
        ValueTask<Func<CancellationToken, ValueTask>> DequeueAsync(CancellationToken cancellationToken);
    }
}

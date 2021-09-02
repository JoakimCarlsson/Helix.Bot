using System;
using System.Threading;
using System.Threading.Tasks;

namespace Helix.BackgroundWorker.Absractions
{
    public interface IBackgroundTaskQueue
    {
        ValueTask QueueAsync(IWorkEvent workEvent);
        ValueTask<IWorkEvent> DequeueAsync(CancellationToken cancellationToken);
    }
}

using System.Threading;
using System.Threading.Tasks;

namespace Helix.Core.Abstractions
{
    public interface IBackgroundTaskQueue
    {
        ValueTask QueueAsync(IWorkEvent workEvent);
        ValueTask<IWorkEvent> DequeueAsync(CancellationToken cancellationToken);
    }
}

using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Helix.Core.Abstractions;

namespace Helix.Core.Services
{
    public class BackgroundTaskQueue : IBackgroundTaskQueue
    {
        private readonly Channel<IWorkEvent> _queue;

        public BackgroundTaskQueue()
        {
            var options = new BoundedChannelOptions(1000)
            {
                FullMode = BoundedChannelFullMode.DropOldest
            };
            _queue = Channel.CreateBounded<IWorkEvent>(options);
        }

        public async ValueTask QueueAsync(IWorkEvent workEvent)
        {
            if (workEvent == null)
                throw new ArgumentNullException(nameof(workEvent));

            await _queue.Writer.WriteAsync(workEvent);
        }

        public async ValueTask<IWorkEvent> DequeueAsync(CancellationToken cancellationToken)
        {
            var workEvent = await _queue.Reader.ReadAsync(cancellationToken);
            return workEvent;
        }
    }
}

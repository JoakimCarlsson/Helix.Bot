using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Helix.BackgroundWorker.Absractions;

namespace Helix.BackgroundWorker.Services
{
    class BackgroundTaskQueue : IBackgroundTaskQueue
    {
        private readonly Channel<Func<CancellationToken, ValueTask>> _queue;

        public BackgroundTaskQueue()
        {
            var options = new BoundedChannelOptions(1000)
            {
                FullMode = BoundedChannelFullMode.DropOldest
            };
            _queue = Channel.CreateBounded<Func<CancellationToken, ValueTask>>(options);
        }

        public async ValueTask QueueAsync(Func<CancellationToken, ValueTask> workEvent)
        {
            if (workEvent == null)
                throw new ArgumentNullException(nameof(workEvent));

            await _queue.Writer.WriteAsync(workEvent);
        }

        public async ValueTask<Func<CancellationToken, ValueTask>> DequeueAsync(CancellationToken cancellationToken)
        {
            var workEvent = await _queue.Reader.ReadAsync(cancellationToken);
            return workEvent;
        }
    }
}

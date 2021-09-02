using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Helix.BackgroundWorker.Absractions;
using Helix.BackgroundWorker.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Helix.BackgroundWorker.Extensions
{
    public static class BackgroundQueueExtensions
    {
        public static IServiceCollection AddBackgroundQueue(this IServiceCollection services)
        {
            services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();
            services.AddHostedService<BackgroundWorkerService>();
            return services;
        }
    }
}

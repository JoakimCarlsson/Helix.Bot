using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Helix.BackgroundWorker.Abstractions;
using Helix.BackgroundWorker.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Helix.BackgroundWorker
{
    public static class DefaultBackgroundWorker
    {
        public static IServiceCollection AddDefaultBackgroundWorker(this IServiceCollection services)
        {
            services.AddSingleton<IWorkQueueService, WorkQueueService>();
            services.AddHostedService<WorkQueueHostedService>();
            return services;
        }
    }
}

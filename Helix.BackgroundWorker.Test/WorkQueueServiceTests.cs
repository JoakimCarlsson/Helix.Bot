using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Helix.BackgroundWorker.Abstractions;
using Helix.BackgroundWorker.Test.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Xunit;

namespace Helix.BackgroundWorker.Test
{
    public class WorkQueueServiceTests
    {
        private readonly IServiceProvider _serviceProvider;

        public WorkQueueServiceTests()
        {
            _serviceProvider = new ServiceCollection()
                .AddLogging()
                .AddScoped<DemoService>()
                .AddDefaultBackgroundWorker()
                .BuildServiceProvider();
        }

        [Fact]
        public async Task MethodShouldFinishBeforeTask()
        {
            //Arrange
            var service = _serviceProvider.GetRequiredService<IHostedService>();
            await service.StartAsync(CancellationToken.None);
            var sut = _serviceProvider.GetRequiredService<IWorkQueueService>();
            var delay = 1000;

            //Act
            var startTime = DateTimeOffset.Now;
            await sut.QueueAsync<DemoService>(async x => await x.MethodThatTakesTimeToExecute(delay));
            var endTime = DateTimeOffset.Now;

            //Assert
            (endTime - startTime).Should().BeLessThan(TimeSpan.FromMilliseconds(delay));
        }

        [Fact]
        public async Task QueueShouldFinishTask()
        {
            //Arrange
            var sut = _serviceProvider.GetRequiredService<IWorkQueueService>();
            var service = _serviceProvider.GetRequiredService<IHostedService>();
            await service.StartAsync(CancellationToken.None);
            int delay = 250;

            //Act
            DemoService.Done.Should().BeFalse();
            await sut.QueueAsync<DemoService>(async x => await x.MethodThatTakesTimeToExecute(delay));
            await Task.Delay(350);
            DemoService.Done.Should().BeTrue();
        }
    }
}
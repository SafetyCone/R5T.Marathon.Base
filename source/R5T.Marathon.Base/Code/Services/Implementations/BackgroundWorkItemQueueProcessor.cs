using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;


namespace R5T.Marathon
{
    // Based on QueuedHostedService from: https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/hosted-services?view=aspnetcore-2.2&tabs=visual-studio#queued-background-tasks-1
    public class BackgroundWorkItemQueueProcessor : BackgroundService
    {
        private IServiceProvider ServiceProvider { get; }
        private IBackgroundWorkItemQueue BackgroundWorkItemQueue { get; }
        private ILogger Logger { get; }


        public BackgroundWorkItemQueueProcessor(
            IServiceProvider serviceProvider,
            IBackgroundWorkItemQueue backgroundWorkItemQueue,
            ILogger<BackgroundWorkItemQueueProcessor> logger)
        {
            this.ServiceProvider = serviceProvider;
            this.BackgroundWorkItemQueue = backgroundWorkItemQueue;
            this.Logger = logger;
        }

        protected async override Task ExecuteAsync(CancellationToken cancellationToken)
        {
            this.Logger.LogInformation("Queued Hosted Service is starting.");

            while (!cancellationToken.IsCancellationRequested)
            {
                var workItem = await this.BackgroundWorkItemQueue.DequeueAsync(cancellationToken);

                try
                {
                    using (var scope = this.ServiceProvider.CreateScope())
                    {
                        await workItem(scope.ServiceProvider, cancellationToken);
                    }
                }
                catch (Exception ex)
                {
                    this.Logger.LogError(ex, $"Error occurred executing {nameof(workItem)}.");
                }
            }

            this.Logger.LogInformation("Queued Hosted Service is stopping.");
        }
    }
}

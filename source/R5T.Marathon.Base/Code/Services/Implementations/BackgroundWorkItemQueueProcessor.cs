using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using R5T.D0049;
using R5T.T0064;


namespace R5T.Marathon
{
    // Based on QueuedHostedService from: https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/hosted-services?view=aspnetcore-2.2&tabs=visual-studio#queued-background-tasks-1
    [ServiceImplementationMarker]
    public class BackgroundWorkItemQueueProcessor : BackgroundService, IServiceImplementation
    {
        private IServiceProvider ServiceProvider { get; }
        private IBackgroundWorkItemQueue BackgroundWorkItemQueue { get; }
        private IExceptionSink ExceptionSink { get; set; }
        private ILogger Logger { get; }


        public BackgroundWorkItemQueueProcessor(
            IServiceProvider serviceProvider,
            IBackgroundWorkItemQueue backgroundWorkItemQueue,
            IExceptionSink exceptionSink,
            ILogger<BackgroundWorkItemQueueProcessor> logger)
        {
            this.ServiceProvider = serviceProvider;
            this.BackgroundWorkItemQueue = backgroundWorkItemQueue;
            this.ExceptionSink = exceptionSink;
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
                catch (Exception exception)
                {
                    this.Logger.LogError(exception, $"Error occurred executing {nameof(workItem)}.");

                    await this.ExceptionSink.Consume(exception);
                }
            }

            this.Logger.LogInformation("Queued Hosted Service is stopping.");
        }
    }
}

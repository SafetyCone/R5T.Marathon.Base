using System;
using System.Threading;
using System.Threading.Tasks;

using R5T.T0064;


namespace R5T.Marathon
{
    [ServiceDefinitionMarker]
    public interface IBackgroundWorkItemQueue : IServiceDefinition
    {
        void QueueBackgroundWorkItem(Func<IServiceProvider, CancellationToken, Task> workItem);
        Task<Func<IServiceProvider, CancellationToken, Task>> DequeueAsync(CancellationToken cancellationToken);
    }
}

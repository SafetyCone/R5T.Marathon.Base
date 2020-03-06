using System;
using System.Threading;
using System.Threading.Tasks;


namespace R5T.Marathon
{
    public interface IBackgroundWorkItemQueue
    {
        void QueueBackgroundWorkItem(Func<IServiceProvider, CancellationToken, Task> workItem);
        Task<Func<IServiceProvider, CancellationToken, Task>> DequeueAsync(CancellationToken cancellationToken);
    }
}

using System;
using System.Threading;
using System.Threading.Tasks;


namespace R5T.Marathon
{
    public interface IBackgroundWorkItemQueue
    {
        void QueueBackgroundWorkItem(Func<CancellationToken, Task> workItem);
        Task<Func<CancellationToken, Task>> DequeueAsync(CancellationToken cancellationToken);
    }
}

using System;
using System.Threading;
using System.Threading.Tasks;


namespace R5T.Marathon
{
    public static class IBackgroundWorkItemQueueExtensions
    {
        public static void Enqueue(this IBackgroundWorkItemQueue backgroundWorkItemQueue, Func<IServiceProvider, CancellationToken, Task> workItem)
        {
            backgroundWorkItemQueue.QueueBackgroundWorkItem(workItem);
        }
    }
}

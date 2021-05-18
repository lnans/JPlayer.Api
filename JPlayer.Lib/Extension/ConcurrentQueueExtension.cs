using System.Collections.Concurrent;
using System.Collections.Generic;

namespace JPlayer.Lib.Extension
{
    public static class ConcurrentQueueExtension
    {
        /// <summary>
        ///     Dequeue all items in the queue
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queue"></param>
        /// <returns></returns>
        public static IEnumerable<T> DequeueAll<T>(this ConcurrentQueue<T> queue)
        {
            while (queue.TryDequeue(out T item))
                yield return item;
        }
    }
}
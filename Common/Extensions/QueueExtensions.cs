using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Common.Extensions
{
    public static class QueueExtensions
    {
        #region Public Methods

        public static IEnumerable<T> GetFirst<T>(this ConcurrentQueue<T> queue)
            where T : class
        {
            if (!queue.IsEmpty)
            {
                queue.TryPeek(out T item);

                if (item != default)
                {
                    yield return item;
                }
            }
        }

        #endregion Public Methods
    }
}
// This file is a part of DiabloSpeech and is under the MIT license.
// See the LICENSE file at the root of the project for more information.
using System;
using System.Threading.Tasks;

namespace DiabloSpeech.Extensions
{
    public static class TaskExtensions
    {
        /// <summary>
        /// Continue task completion on the current "caller" thread.
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        public static Task ContinueCurrent(this Task task, Action<Task> continuationAction)
        {
            return task.ContinueWith(continuationAction, TaskScheduler.FromCurrentSynchronizationContext());
        }

        /// <summary>
        /// Continue task completion on the current "caller" thread.
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        public static Task ContinueCurrent<T>(this Task<T> task, Action<Task<T>> continuationAction)
        {
            return task.ContinueWith(continuationAction, TaskScheduler.FromCurrentSynchronizationContext());
        }
    }
}

// This file is a part of DiabloSpeech and is under the MIT license.
// See the LICENSE file at the root of the project for more information.
using System.Collections.Generic;

namespace DiabloSpeech.Extensions
{
    public static class ListExtensions
    {
        /// <summary>
        /// Gets the element at the specified index or default.
        /// </summary>
        /// <typeparam name="T">Value type.</typeparam>
        /// <param name="list">Array or list.</param>
        /// <param name="index">Index accessor.</param>
        /// <returns>The element with the specified index or default.</returns>
        public static T ValueOrDefault<T>(this IReadOnlyList<T> list, int index)
        {
            if (index < 0 || index >= list.Count)
                return default(T);
            return list[index];
        }
    }
}

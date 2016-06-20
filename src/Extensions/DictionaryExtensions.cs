// This file is a part of DiabloSpeech and is under the MIT license.
// See the LICENSE file at the root of the project for more information.
using System.Collections.Generic;

namespace DiabloSpeech.Extensions
{
    public static class DictionaryExtensions
    {
        /// <summary>
        /// Gets the element at with the specified key or default.
        /// </summary>
        /// <typeparam name="T">Key type.</typeparam>
        /// <typeparam name="U">Value type.</typeparam>
        /// <param name="dict">Dictionary target.</param>
        /// <param name="key">Dictionary key.</param>
        /// <returns>The element with the specified key or default.</returns>
        public static U ValueOrDefault<T, U>(this IReadOnlyDictionary<T, U> dict, T key)
        {
            U value;
            dict.TryGetValue(key, out value);
            return value;
        }
    }
}

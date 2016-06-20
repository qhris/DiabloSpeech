// This file is a part of DiabloSpeech and is under the MIT license.
// See the LICENSE file at the root of the project for more information.
namespace DiabloSpeech.Extensions
{
    public static class StringExtensions
    {
        public static string CapitalizeFirst(this string value)
        {
            if (string.IsNullOrEmpty(value)) return "";
            return char.ToUpperInvariant(value[0]) + value.Substring(1);
        }
    }
}

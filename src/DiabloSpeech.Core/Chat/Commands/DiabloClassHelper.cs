// This file is a part of DiabloSpeech and is under the MIT license.
// See the LICENSE file at the root of the project for more information.
namespace DiabloSpeech.Core.Chat.Commands
{
    static class DiabloClassHelper
    {
        /// <summary>
        /// Resolve input class name abbreviations.
        /// </summary>
        /// <param name="name">Full class name or null.</param>
        /// <returns></returns>
        public static string ResolveClassName(string name)
        {
            switch (name.ToLowerInvariant())
            {
                case "ama":
                case "amazon":
                    return "amazon";
                case "assa":
                case "assassin":
                case "sin":
                    return "assassin";
                case "barb":
                case "baba":
                case "useless":
                case "barbarian":
                    return "barbarian";
                case "druid":
                case "dudu":
                    return "druid";
                case "necro":
                case "necromancer":
                    return "necromancer";
                case "pala":
                case "pally":
                case "paladin":
                    return "paladin";
                case "sorc":
                case "sorceress":
                    return "sorceress";
                default: return null;
            }
        }
    }
}

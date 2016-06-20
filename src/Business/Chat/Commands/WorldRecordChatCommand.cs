// This file is a part of DiabloSpeech and is under the MIT license.
// See the LICENSE file at the root of the project for more information.
using DiabloSpeech.Business.Speedrun;
using DiabloSpeech.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiabloSpeech.Business.Chat.Commands
{
    [CommandAlias("wr", "record", "records")]
    class WorldRecordChatCommand : IChatCommand
    {
        public async Task Process(IChatWriter chat, ChatCommandData data)
        {
            if (data.Arguments.Count == 0)
            {
                chat.SendMessage($"Usage !{data.CommandAlias} [class]. Values: amazon, assassin, barbarian, druid, necromancer, paladin, sorceress, and some abbreviations.");
                return;
            }

            string characterClass = ResolveClassName(data.Arguments[0].ToLowerInvariant());
            if (string.IsNullOrEmpty(characterClass))
            {
                chat.SendMessage($"Invalid class name.");
                return;
            }

            // Get the current leaderboard for the specified class.
            var client = new SpeedrunClient();
            var leaderboard = await client.QueryLeaderboardAsync();
            IList<GameRecord> records = leaderboard.ValueOrDefault(characterClass);

            // Format records for printing.
            string message = string.Join(", ",
                from record in records
                let time = record.Time.ToString(@"hh\:mm\:ss")
                select $"{record.Category} in {time} [{record.User}]");

            if (records.Count == 0)
            {
                message = "No records available.";
            }

            message = $"{ characterClass.CapitalizeFirst() } records: {message}";
            chat.SendMessage(message);
        }

        string ResolveClassName(string name)
        {
            switch (name)
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

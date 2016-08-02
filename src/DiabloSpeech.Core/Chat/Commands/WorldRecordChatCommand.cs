// This file is a part of DiabloSpeech and is under the MIT license.
// See the LICENSE file at the root of the project for more information.
using DiabloSpeech.Core.Speedrun;
using DiabloSpeech.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiabloSpeech.Core.Chat.Commands
{
    [CommandAlias("wr", "record", "records")]
    class WorldRecordChatCommand : IChatCommand
    {
        public bool IsModeratorCommand { get { return false; } }

        public async Task Process(IChatWriter chat, ChatCommandData data)
        {
            if (data.Arguments.Count == 0)
            {
                chat.SendMessage($"Usage !{data.CommandAlias} [class]. Values: amazon, assassin, barbarian, druid, necromancer, paladin, sorceress, and some abbreviations.");
                return;
            }

            string characterClass = DiabloClassHelper.ResolveClassName(data.Arguments[0]);
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
    }
}

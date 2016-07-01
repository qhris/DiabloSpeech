// This file is a part of DiabloSpeech and is under the MIT license.
// See the LICENSE file at the root of the project for more information.
using DiabloSpeech.Extensions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DiabloSpeech.Business.Chat.Commands
{
    [CommandAlias("breakpoint", "breakpoints")]
    class BreakpointsChatCommand : IChatCommand
    {
        static Dictionary<string, Dictionary<string, string>> BreakpointsData =>
            new Dictionary<string, Dictionary<string, string>>()
        {
            ["fcr"] = new Dictionary<string, string>()
            {
                ["amazon"] = "0 7 14 22 32 48 68 99 152",
                ["assassin"] = "0 8 16 27 42 65 102 174",
                ["barbarian"] = "0 9 20 37 63 105 200",
                ["druid"] = "0 4 10 19 30 46 68 99 163",
                ["necromancer"] = "0 9 18 30 48 75 125",
                ["paladin"] = "0 9 18 30 48 75 125",
                ["sorceress"] = "0 9 20 37 63 105 200",
            },
            ["fhr"] = new Dictionary<string, string>()
            {
                ["amazon"] = "0 6 13 20 32 52 86 174 600",
                ["assassin"] = "0 7 15 27 48 86 200",
                ["barbarian"] = "0 7 15 27 48 86 200",
                ["druid"] = "1H: 0 3 7 13 19 29 42 63 99 174 456, Other: 0 5 10 16 26 39 56 86 152 360",
                ["necromancer"] = "0 50 10 16 26 39 56 86 152 377",
                ["paladin"] = "0 7 15 27 48 86 200",
                ["sorceress"] = "0 5 9 14 20 30 42 60 86 142 280",
            }
        };

        public async Task Process(IChatWriter chat, ChatCommandData data)
        {
            if (chat == null)
                throw new ArgumentNullException(nameof(chat));
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            if (data.Arguments.Count < 2)
            {
                chat.SendMessage($"Usage: !{data.CommandAlias} [type] [class]. Type values: fcr, fhr. Example: !{data.CommandAlias} fcr sorceress");
                return;
            }

            // Find breakpoint lookup type.
            string lookupType = data.Arguments[0].ToLowerInvariant();
            if (lookupType != "fcr" && lookupType != "fhr")
            {
                chat.SendMessage($"Invalid breakpoint type.");
                return;
            }

            // Find class to lookup breakpoints for.
            string characterClass = DiabloClassHelper.ResolveClassName(data.Arguments[1]);
            if (string.IsNullOrEmpty(characterClass))
            {
                chat.SendMessage($"Invalid class name.");
                return;
            }

            string breakpoints = Breakpoints(lookupType,characterClass);
            if (string.IsNullOrEmpty(breakpoints))
            {
                chat.SendMessage("No breakpoints found.");
                return;
            }

            chat.SendMessage($"{characterClass.CapitalizeFirst()} {lookupType.ToUpperInvariant()} breakpoints: [{breakpoints}].");

            // Avoid the warning about async running synchronously.
            await FinalizeAsync;
        }

        string Breakpoints(string type, string @class) =>
            BreakpointsData.ValueOrDefault(type)?.ValueOrDefault(@class);

        Task<int> FinalizeAsync => Task.FromResult(0);
    }
}

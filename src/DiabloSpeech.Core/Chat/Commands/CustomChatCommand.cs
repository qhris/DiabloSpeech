// This file is a part of DiabloSpeech and is under the MIT license.
// See the LICENSE file at the root of the project for more information.
using DiabloSpeech.Extensions;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using DiabloSpeech.Core.Chat.Data;

namespace DiabloSpeech.Core.Chat.Commands
{
    [CommandAlias("command", "cmd")]
    class CustomChatCommand : IChatCommand, ICustomCommandProcessor
    {
        Task<int> FinalizeAsync => Task.FromResult(0);

        public bool IsModeratorCommand { get { return true; } }
        public CustomCommandCollection CommandCollection { get; set; }

        Dictionary<string, Action<IChatWriter, ChatCommandData>> subCommands;

        public CustomChatCommand()
        {
            subCommands = new Dictionary<string, Action<IChatWriter, ChatCommandData>>()
            {
                ["add"] = AddCommand,
                ["remove"] = RemoveCommand,
                ["list"] = ListCommand,
            };
        }

        public async Task Process(IChatWriter chat, ChatCommandData data)
        {
            if (chat == null)
                throw new ArgumentNullException(nameof(chat));
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            if (CommandCollection == null)
            {
                chat.SendMessage("Error: Unable to operate on custom commands.");
                return;
            }

            Console.WriteLine($"Arguments: {string.Join(", ", data.Arguments)}.");

            string subCommand = data.Arguments.ValueOrDefault(0)?.ToLowerInvariant();
            if (subCommand == null || !subCommands.ContainsKey(subCommand))
            {
                string commands = string.Join("|", subCommands.Keys);
                chat.SendMessage($"Usage: !{data.CommandAlias} <{commands}>. Was; {subCommand}");
            }
            // Execute subcommand if found.
            else subCommands[subCommand](chat, data);

            await FinalizeAsync;
        }

        void AddCommand(IChatWriter chat, ChatCommandData data)
        {
            if (data.Arguments.Count < 3)
            {
                chat.SendMessage($"Add command. Usage: !{data.CommandAlias} add <command> !<optional subcommand> <text>.");
                return;
            }

            string command = data.Arguments[1].ToLowerInvariant();
            string subCommand = data.Arguments[2].ToLowerInvariant();
            string text = data.Arguments.ValueOrDefault(3)?.ToLowerInvariant();

            bool isSubCommand = text != null && subCommand.StartsWith("!");
            text = string.Join(" ", data.Arguments.Skip(isSubCommand ? 3 : 2));

            // If only 2 arguments are passed to this subcommand.
            if (isSubCommand)
            {
                subCommand = subCommand.TrimStart('!');
                CommandCollection.AddSubCommand(command, subCommand, text);
                chat.SendMessage($"Added sub command '{command} {subCommand}'.");
            }
            else
            {
                CommandCollection.AddCommand(command, text);
                chat.SendMessage($"Added command '{command}'.");
            }
        }

        void RemoveCommand(IChatWriter chat, ChatCommandData data)
        {
            if (data.Arguments.Count < 2)
            {
                chat.SendMessage($"Remove command. Usage: !{data.CommandAlias} remove <command> <optional subcommand>.");
                return;
            }

            string command = data.Arguments[1].ToLowerInvariant();
            string subCommand = data.Arguments.ValueOrDefault(2)?.ToLowerInvariant();

            if (subCommand == null)
            {
                CommandCollection.RemoveCommand(command);
                chat.SendMessage($"Removed command '{command}'.");
            }
            else
            {
                CommandCollection.RemoveSubCommand(command, subCommand);
                chat.SendMessage($"Removed sub command '{command} {subCommand}'.");
            }
        }

        void ListCommand(IChatWriter chat, ChatCommandData data)
        {
            if (CommandCollection == null)
            {
                chat.SendMessage("Error: Unable to operate on custom commands.");
                return;
            }

            if (CommandCollection.Commands.Count == 0)
            {
                chat.SendMessage("No custom commands added.");
                return;
            }

            string commands = string.Join(", ", CommandCollection.Commands.Keys);
            chat.SendMessage($"Available commands: {commands}.");
        }
    }
}

// This file is a part of DiabloSpeech and is under the MIT license.
// See the LICENSE file at the root of the project for more information.
using System;
using System.Collections.Generic;

namespace DiabloSpeech.Core.Chat.Data
{
    public class CustomCommandCollection
    {
        Dictionary<string, CustomChatCommandData> commands;

        public event Action CollectionModified;

        public IReadOnlyDictionary<string, CustomChatCommandData> Commands {  get { return commands; } }

        public CustomCommandCollection()
        {
            commands = new Dictionary<string, CustomChatCommandData>();
        }

        public void AddCommand(string command, string text)
        {
            if (string.IsNullOrWhiteSpace(command))
                throw new ArgumentNullException(nameof(command));
            if (string.IsNullOrEmpty(text))
                throw new ArgumentNullException(nameof(text));

            var customCommand = GetOrAddCommand(command, text);
            customCommand.Text = text;
            OnCollectionModified();
        }

        public void AddSubCommand(string command, string subCommand, string text)
        {
            if (string.IsNullOrWhiteSpace(command))
                throw new ArgumentNullException(nameof(command));
            if (string.IsNullOrWhiteSpace(subCommand))
                throw new ArgumentNullException(nameof(subCommand));
            if (string.IsNullOrEmpty(text))
                throw new ArgumentNullException(nameof(text));

            var customCommand = GetOrAddCommand(command);

            // Add or overwrite commands.
            subCommand = subCommand.ToLowerInvariant();
            customCommand.Subcommands[subCommand] = text;
            OnCollectionModified();
        }

        CustomChatCommandData GetOrAddCommand(string command, string text = null)
        {
            command = command.ToLowerInvariant();
            CustomChatCommandData customCommand;
            if (!commands.TryGetValue(command, out customCommand))
            {
                customCommand = new CustomChatCommandData()
                {
                    Name = command,
                    Text = text,
                    Subcommands = new Dictionary<string, string>()
                };

                commands[command] = customCommand;
            }

            return customCommand;
        }

        public void RemoveCommand(string command)
        {
            if (string.IsNullOrWhiteSpace(command))
                throw new ArgumentNullException(nameof(command));

            if (commands.ContainsKey(command))
            {
                commands.Remove(command);
                OnCollectionModified();
            }
        }

        public void RemoveSubCommand(string command, string subCommand)
        {
            if (string.IsNullOrWhiteSpace(command))
                throw new ArgumentNullException(command);
            if (string.IsNullOrWhiteSpace(subCommand))
                throw new ArgumentNullException(subCommand);

            command = command.ToLowerInvariant();
            subCommand = subCommand.ToLowerInvariant();

            CustomChatCommandData customCommand;
            if (!commands.TryGetValue(command, out customCommand))
                return;

            if (customCommand.Subcommands.ContainsKey(subCommand))
            {
                customCommand.Subcommands.Remove(subCommand);
                OnCollectionModified();
            }
        }

        public bool ContainsCommand(string command)
        {
            if (string.IsNullOrWhiteSpace(command))
                throw new ArgumentNullException(nameof(command));
            command = command.ToLowerInvariant();

            return commands.ContainsKey(command);
        }

        public bool ContainsSubCommand(string command, string subCommand)
        {
            if (string.IsNullOrWhiteSpace(command))
                throw new ArgumentNullException(nameof(command));
            if (string.IsNullOrWhiteSpace(subCommand))
                throw new ArgumentNullException(nameof(subCommand));

            command = command.ToLowerInvariant();
            subCommand = subCommand.ToLowerInvariant();

            CustomChatCommandData customCommand;
            if (!commands.TryGetValue(command, out customCommand))
                return false;
            return customCommand.Subcommands.ContainsKey(subCommand);
        }

        protected void OnCollectionModified() =>
            CollectionModified?.Invoke();
    }
}

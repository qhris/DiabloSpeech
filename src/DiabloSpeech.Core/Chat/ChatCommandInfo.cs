// This file is a part of DiabloSpeech and is under the MIT license.
// See the LICENSE file at the root of the project for more information.
using System.Collections.Generic;

namespace DiabloSpeech.Core.Chat
{
    public class ChatCommandInfo
    {
        public string Name { get; }
        public IChatCommand Instance { get; }
        public IReadOnlyList<string> Aliases { get; }

        public ChatCommandInfo(string name, IChatCommand instance, IReadOnlyList<string> aliases)
        {
            Name = name;
            Instance = instance;
            Aliases = aliases;
        }
    }
}

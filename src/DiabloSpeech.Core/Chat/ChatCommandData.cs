// This file is a part of DiabloSpeech and is under the MIT license.
// See the LICENSE file at the root of the project for more information.
using System.Collections.Generic;

namespace DiabloSpeech.Core.Chat
{
    public class ChatCommandData
    {
        public IReadOnlyList<string> Arguments { get; }
        public string CommandAlias { get; }

        public ChatCommandData(string commandAlias, IReadOnlyList<string> arguments)
        {
            CommandAlias = commandAlias;
            Arguments = arguments;
        }
    }
}

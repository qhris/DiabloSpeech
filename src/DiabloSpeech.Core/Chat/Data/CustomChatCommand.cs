// This file is a part of DiabloSpeech and is under the MIT license.
// See the LICENSE file at the root of the project for more information.
using System.Collections.Generic;

namespace DiabloSpeech.Core.Chat.Data
{
    public class CustomChatCommandData
    {
        public string Name { get; set; }
        public string Text { get; set; }
        public Dictionary<string, string> Subcommands { get; set; }
    }
}

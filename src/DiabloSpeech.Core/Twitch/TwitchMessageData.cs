// This file is a part of DiabloSpeech and is under the MIT license.
// See the LICENSE file at the root of the project for more information.
using System.Collections.Generic;

namespace DiabloSpeech.Core.Twitch
{
    public class TwitchMessageData
    {
        public string Raw { get; set; }
        public Dictionary<string, string> Tags { get; set; } = new Dictionary<string, string>();
        public string Prefix { get; set; }
        public string Command { get; set; }
        public List<string> Params { get; set; } = new List<string>();
    }
}

// This file is a part of DiabloSpeech and is under the MIT license.
// See the LICENSE file at the root of the project for more information.
using System;
using System.Collections.Generic;

namespace DiabloSpeech.Core.Speedrun
{
    internal class LeaderboardPlayer
    {
        public string Uri { get; set; }
        public string Id { get; set; }
    }

    internal class LeaderboardTime
    {
        public int Realtime_t { get; set; }
    }

    internal class LeaderboardRun
    {
        public string Id { get; set; }
        public string Game { get; set; }
        public string Category { get; set; }
        public string Comment { get; set; }
        public IList<LeaderboardPlayer> Players { get; set; }
        public DateTime? Date { get; set; }
        public DateTime? Submitted { get; set; }
        public LeaderboardTime Times { get; set; }
        public Dictionary<string, string> Values { get; set; }
    }
}

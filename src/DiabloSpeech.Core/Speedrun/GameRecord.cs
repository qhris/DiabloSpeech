// This file is a part of DiabloSpeech and is under the MIT license.
// See the LICENSE file at the root of the project for more information.
using System;

namespace DiabloSpeech.Core.Speedrun
{
    public class GameRecord
    {
        public string User { get; set; }
        public string Category { get; set; }
        public TimeSpan Time { get; set; }
    }
}

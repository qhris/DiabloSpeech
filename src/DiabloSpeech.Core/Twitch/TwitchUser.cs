// This file is a part of DiabloSpeech and is under the MIT license.
// See the LICENSE file at the root of the project for more information.
using System.Windows.Media;

namespace DiabloSpeech.Core.Twitch
{
    public class TwitchUser
    {
        public string Name { get; }
        public Color Color { get; set; }
        public bool IsModerator { get; }

        public TwitchUser(string name, bool moderator)
        {
            Name = name;
            IsModerator = moderator;
            Color = Colors.Black;
        }

        public TwitchUser(string name, bool moderator, Color color)
        {
            Name = name;
            IsModerator = moderator;
            Color = color;
        }
    }
}

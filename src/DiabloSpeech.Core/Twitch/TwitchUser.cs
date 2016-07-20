// This file is a part of DiabloSpeech and is under the MIT license.
// See the LICENSE file at the root of the project for more information.
using System.Windows.Media;

namespace DiabloSpeech.Core.Twitch
{
    public class TwitchUser
    {
        public string Name { get; private set; }
        public Color Color { get; set; }

        public TwitchUser(string name)
        {
            Name = name;
            Color = Colors.Black;
        }

        public TwitchUser(string name, Color color)
        {
            Name = name;
            Color = color;
        }
    }
}

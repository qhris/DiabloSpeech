// This file is a part of DiabloSpeech and is under the MIT license.
// See the LICENSE file at the root of the project for more information.
using DiabloSpeech.Extensions;
using System;
using System.Windows.Media;

namespace DiabloSpeech.Core.Twitch.Processors
{
    public class UserStateCommandProcessor : ITwitchCommandProcessor
    {
        public event Action<TwitchUser> AcquireUserState;

        public void Process(ITwitchChannelConnection connection, TwitchMessageData data)
        {
            if (connection == null)
                throw new ArgumentNullException(nameof(connection));
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            string channel = data.Params.ValueOrDefault(0) ?? connection.Channel;
            if (channel != connection.Channel) return;

            // Get formatted user information if possible.
            string name = data.Tags.ValueOrDefault("display-name") ?? connection.Username;
            string colorText = data.Tags.ValueOrDefault("color") ?? "Black";

            // Try to get user color.
            Color color = Colors.Black;
            try { color = (Color)ColorConverter.ConvertFromString(colorText); }
            catch (FormatException) { }

            var user = new TwitchUser(name, true, color);

            OnAcquireUserState(user);
        }

        protected virtual void OnAcquireUserState(TwitchUser user) =>
            AcquireUserState?.Invoke(user);
    }
}

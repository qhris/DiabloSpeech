// This file is a part of DiabloSpeech and is under the MIT license.
// See the LICENSE file at the root of the project for more information.
using DiabloSpeech.Extensions;
using System;
using System.Windows.Media;

namespace DiabloSpeech.Business.Twitch.Processors
{
    public class UserStateCommandProcessor : ITwitchCommandProcessor
    {
        public event Action<TwitchUser> AcquireUserState;

        public void Process(ITwitchChannelConnection connection, TwitchMessageData data)
        {
            string channel = data.Params.ValueOrDefault(0) ?? connection.Channel;
            if (channel != connection.Channel) return;

            // Get formatted user information if possible.
            string name = data.Tags.ValueOrDefault("display-name") ?? connection.Username;
            string color = data.Tags.ValueOrDefault("color") ?? "Black";
            var user = new TwitchUser(name, (Color)ColorConverter.ConvertFromString(color));

            OnAcquireUserState(user);
        }

        protected virtual void OnAcquireUserState(TwitchUser user) =>
            AcquireUserState?.Invoke(user);
    }
}

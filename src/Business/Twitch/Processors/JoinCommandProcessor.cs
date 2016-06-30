// This file is a part of DiabloSpeech and is under the MIT license.
// See the LICENSE file at the root of the project for more information.
using DiabloSpeech.Extensions;
using System;

namespace DiabloSpeech.Business.Twitch.Processors
{
    public class JoinCommandProcessor : ITwitchCommandProcessor
    {
        public event Action<string, string> Joined;

        public void Process(ITwitchChannelConnection connection, TwitchMessageData data)
        {
            if (connection == null)
                throw new ArgumentNullException(nameof(connection));
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            string channel = data.Params.ValueOrDefault(0);
            string username = data.Prefix?.Split('!').ValueOrDefault(0);
            if (username == null) return;

            OnUserJoined(channel, username.TrimStart(':'));
        }

        protected virtual void OnUserJoined(string channel, string username) =>
            Joined?.Invoke(channel, username);
    }
}

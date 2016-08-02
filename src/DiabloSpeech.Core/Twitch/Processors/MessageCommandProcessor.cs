// This file is a part of DiabloSpeech and is under the MIT license.
// See the LICENSE file at the root of the project for more information.
using DiabloSpeech.Extensions;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Media;

namespace DiabloSpeech.Core.Twitch.Processors
{
    public class MessageCommandProcessor : ITwitchCommandProcessor
    {
        public event Action<TwitchChatMessage> MessageReceived;

        public void Process(ITwitchChannelConnection connection, TwitchMessageData data)
        {
            if (connection == null)
                throw new ArgumentNullException(nameof(connection));
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            // Extract message data.
            string name = data.Prefix?.Split('!')[0] ?? "Unknown";
            string channel = data.Params.ValueOrDefault(0) ?? connection.Channel;
            string message = data.Params.ValueOrDefault(1);
            if (message == null) return;

            // Message coming in on the wrong channel.
            if (channel != connection.Channel)
                return;

            // Get formatted user information if possible.
            string displayName = data.Tags.ValueOrDefault("display-name");
            name = string.IsNullOrEmpty(displayName) ? name.CapitalizeFirst() : displayName;
            string colorText = data.Tags.ValueOrDefault("color") ?? "Black";

            // Action messages "/me <message>"...
            var type = TwitchChatMessageType.Normal;
            var match = Regex.Match(message, @"^\u0001ACTION ([^\u0001]+)\u0001$");
            if (match.Success)
            {
                message = match.Groups[1].Value;
                type = TwitchChatMessageType.Self;
            }

            // Try to get the users color or default to black.
            Color color = Colors.Black;
            try { color = (Color)ColorConverter.ConvertFromString(colorText); }
            catch (FormatException) { }

            // Check for moderator status.
            bool moderator = data.Tags.ValueOrDefault("mod") == "1";
            moderator |= data.Tags.ValueOrDefault("badges")?.Contains("broadcaster") ?? false;

            var user = new TwitchUser(name, moderator, color);
            OnMessageReceived(new TwitchChatMessage(type, user, message));
        }

        protected virtual void OnMessageReceived(TwitchChatMessage message) =>
            MessageReceived?.Invoke(message);
    }
}

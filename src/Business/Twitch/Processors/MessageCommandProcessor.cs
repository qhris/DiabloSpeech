﻿// This file is a part of DiabloSpeech and is under the MIT license.
// See the LICENSE file at the root of the project for more information.
using DiabloSpeech.Extensions;
using System;
using System.Text.RegularExpressions;
using System.Windows.Media;

namespace DiabloSpeech.Business.Twitch.Processors
{
    public class MessageCommandProcessor : ITwitchCommandProcessor
    {
        public event Action<TwitchChatMessage> MessageReceived;

        public void Process(ITwitchChannelConnection connection, TwitchMessageData data)
        {
            string name = data.Prefix.Split('!')[0];
            string channel = data.Params.ValueOrDefault(0);
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

            var user = new TwitchUser(name, color);
            OnMessageReceived(new TwitchChatMessage(type, user, message));
        }

        protected virtual void OnMessageReceived(TwitchChatMessage message) =>
            MessageReceived?.Invoke(message);
    }
}

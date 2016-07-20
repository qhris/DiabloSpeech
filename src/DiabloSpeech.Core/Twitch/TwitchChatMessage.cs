// This file is a part of DiabloSpeech and is under the MIT license.
// See the LICENSE file at the root of the project for more information.
namespace DiabloSpeech.Core.Twitch
{
    public enum TwitchChatMessageType
    {
        Normal,
        Self,
    }

    public class TwitchChatMessage
    {
        public TwitchChatMessageType Type { get; }
        public TwitchUser User { get; }
        public string Text { get; }

        public TwitchChatMessage(TwitchChatMessageType type, TwitchUser user, string message)
        {
            Type = type;
            User = user;
            Text = message;
        }
    }
}

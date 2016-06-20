// This file is a part of DiabloSpeech and is under the MIT license.
// See the LICENSE file at the root of the project for more information.
using DiabloSpeech.Business.Chat;
using DiabloSpeech.Business.Twitch;

namespace DiabloSpeech.Business
{
    public class TwitchChatWriter : IChatWriter
    {
        ITwitchChannelConnection connection;

        public TwitchChatWriter(ITwitchChannelConnection connection)
        {
            this.connection = connection;
        }

        public void SendMessage(string message)
        {
            connection.Send(message);
        }

        public void SendMessage(string format, params string[] args)
        {
            SendMessage(string.Format(format, args));
        }
    }
}

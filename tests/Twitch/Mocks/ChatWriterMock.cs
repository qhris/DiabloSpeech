// This file is a part of DiabloSpeech and is under the MIT license.
// See the LICENSE file at the root of the project for more information.
using DiabloSpeech.Business.Chat;

namespace Tests.Twitch.Mocks
{
    public class ChatWriterMock : IChatWriter
    {
        public string PreviousMessage { get; private set; }

        public void SendMessage(string message)
        {
            PreviousMessage = message;
        }

        public void SendMessage(string format, params string[] args)
        {
            SendMessage(string.Format(format, args));
        }
    }
}

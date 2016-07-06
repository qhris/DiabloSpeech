// This file is a part of DiabloSpeech and is under the MIT license.
// See the LICENSE file at the root of the project for more information.
using DiabloSpeech.Business.Twitch;
using System.Collections.Generic;

namespace Tests.Twitch.Mocks
{
    class TwitchConnectionMock : ITwitchChannelConnection
    {
        public string Channel { get; }
        public string Username { get; }

        public List<string> FiredCommands { get; }

        public TwitchConnectionMock(string username, string channel)
        {
            Username = username;
            Channel = channel;
            FiredCommands = new List<string>();
        }

        public void Close()
        {
        }

        public void Command(string command)
        {
            FiredCommands.Add(command);
        }

        public void Command(string format, params object[] args)
        {
            Command(string.Format(format, args));
        }

        public void Dispose()
        {
        }

        public void Flush()
        {
            FiredCommands.Add("__FLUSH");
        }

        public string Read()
        {
            return null;
        }

        public void Send(string message)
        {
        }

        public void Send(string format, params object[] args)
        {
        }
    }
}

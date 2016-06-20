// This file is a part of DiabloSpeech and is under the MIT license.
// See the LICENSE file at the root of the project for more information.
using System;

namespace DiabloSpeech.Business.Twitch
{
    public interface ITwitchChannelConnection : IDisposable
    {
        string Channel { get; }
        string Username { get; }

        void Close();
        void Command(string command);
        void Command(string format, params object[] args);
        void Flush();
        string Read();
        void Send(string message);
        void Send(string format, params object[] args);
    }
}

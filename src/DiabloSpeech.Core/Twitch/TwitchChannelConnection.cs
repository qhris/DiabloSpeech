// This file is a part of DiabloSpeech and is under the MIT license.
// See the LICENSE file at the root of the project for more information.
using System;
using System.IO;

namespace DiabloSpeech.Core.Twitch
{
    public class TwitchChannelConnection : IDisposable, ITwitchChannelConnection
    {
        INetworkStream networkStream;
        StreamWriter writer;
        StreamReader reader;

        object writerLock = new object();
        object readerLock = new object();

        public string Username { get; private set; }
        public string Channel { get; private set; }

        public TwitchChannelConnection(INetworkStream stream, TwitchAuthenticationDetails authentication)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));
            if (string.IsNullOrEmpty(authentication.Username))
                throw new ArgumentException(nameof(authentication.Username));
            if (string.IsNullOrEmpty(authentication.Password))
                throw new ArgumentException(nameof(authentication.Password));
            if (string.IsNullOrEmpty(authentication.Channel))
                throw new ArgumentException(nameof(authentication.Channel));

            Username = authentication.Username;

            // Channel names are lower case on twitch.
            Channel = authentication.Channel.Trim().ToLowerInvariant();
            if (!Channel.StartsWith("#"))
            {
                Channel = "#" + Channel.ToLowerInvariant();
            }

            try
            {
                networkStream = stream;
                reader = new StreamReader(networkStream.BaseStream);
                writer = new StreamWriter(networkStream.BaseStream);

                Command("PASS {0}", authentication.Password);
                Command("NICK {0}", Username.ToLowerInvariant());
                Command("JOIN {0}", Channel);
                Command("CAP REQ :twitch.tv/membership");
                Command("CAP REQ :twitch.tv/commands");
                Command("CAP REQ :twitch.tv/tags");

                Flush();
            }
            catch (Exception)
            {
                // Make sure we clean up everything even if we throw
                // inside the constructor.
                Dispose(true);

                throw;
            }
        }

        void IDisposable.Dispose()
        {
            Close();
        }

        public void Close()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (networkStream != null)
                {
                    networkStream.Close();
                    networkStream = null;
                }
            }
        }

        public void Flush()
        {
            lock (writerLock)
            {
                writer.Flush();
            }
        }

        public void Command(string command)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            // No newlines in a single command.
            command = command.Replace("\n", "");

            lock (writerLock)
            {
                writer.Write(command + "\r\n");
            }
        }

        public void Command(string format, params object[] args)
        {
            Command(string.Format(format, args));
        }

        public void Send(string message)
        {
            // Ignore empty messages.
            if (string.IsNullOrEmpty(message))
                return;

            Command("PRIVMSG {0} :{1}", Channel, message);
            Flush();
        }

        public void Send(string format, params object[] args)
        {
            Send(string.Format(format, args));
        }

        public string Read()
        {
            lock (readerLock)
            {
                return reader.ReadLine();
            }
        }
    }
}

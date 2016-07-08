// This file is a part of DiabloSpeech and is under the MIT license.
// See the LICENSE file at the root of the project for more information.
using System;
using System.IO;
using System.Net.Sockets;

namespace DiabloSpeech.Business.Twitch
{
    public class TwitchChannelConnection : IDisposable, ITwitchChannelConnection
    {
        TcpClient client;
        StreamWriter writer;
        StreamReader reader;

        object writerLock = new object();
        object readerLock = new object();

        public string Username { get; private set; }
        public string Channel { get; private set; }

        public TwitchChannelConnection(string username, string channel, string password)
        {
            if (string.IsNullOrEmpty(username))
                throw new ArgumentNullException(nameof(username));
            if (string.IsNullOrEmpty(channel))
                throw new ArgumentNullException(nameof(channel));
            if (password == null || password.Length == 0)
                throw new ArgumentNullException(nameof(password));

            Username = username;

            // Channel names are lower case on twitch.
            channel = channel.Trim().ToLowerInvariant();
            if (!channel.StartsWith("#"))
            {
                channel = "#" + channel.ToLowerInvariant();
            }

            Channel = channel;

            try
            {
                client = new TcpClient("irc.chat.twitch.tv", 6667);
                reader = new StreamReader(client.GetStream());
                writer = new StreamWriter(client.GetStream());

                Command("PASS {0}", password);
                Command("NICK {0}", username.ToLowerInvariant());
                Command("JOIN {0}", channel);
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
                if (client != null)
                {
                    client.Close();
                    client = null;
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

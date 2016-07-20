// This file is a part of DiabloSpeech and is under the MIT license.
// See the LICENSE file at the root of the project for more information.
using DiabloSpeech.Core.Twitch.Processors;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace DiabloSpeech.Core.Twitch
{
    public class TwitchClient : IDisposable
    {
        Task runningTask;
        Dictionary<string, ITwitchCommandProcessor> commandProcessors;
        HashSet<string> ignoredCommands;

        public ITwitchChannelConnection Connection { get; private set; }

        /// <summary>
        /// UI (caller) thread dispatcher.
        /// </summary>
        Dispatcher Dispatcher { get; set; }

        /// <summary>
        /// A user has just joined the chat room.
        /// </summary>
        public event Action<string, string> JoinedRoom;

        /// <summary>
        /// Message was just received from a user.
        /// </summary>
        public event Action<TwitchChatMessage> MessageReceived;

        /// <summary>
        /// User state acquired for the bot.
        /// </summary>
        public event Action<TwitchUser> AcquireUserState;

        /// <summary>
        /// Client is successfully connected to a chat room.
        /// </summary>
        public event Action Connected;

        /// <summary>
        /// Client lost connection.
        /// </summary>
        public event Action Disconnected;

        /// <summary>
        /// Called for unhandled IRC messages.
        /// </summary>
        public event Action<TwitchMessageData> UnhandledMessage;

        public TwitchClient(ITwitchChannelConnection connection)
        {
            if (connection == null)
                throw new ArgumentNullException(nameof(connection));
            Dispatcher = Dispatcher.CurrentDispatcher;

            Connection = connection;
            InitializeIgnoredCommands();
            InitializeCommandProcessors();
        }

        void InitializeIgnoredCommands()
        {
            ignoredCommands = new HashSet<string>()
            {
                "001",
                "002",
                "003",
                "004",
                "353",
                "366",
                "372",
                "375",
                "376",
                "CAP",
                "MODE",
                "PART",
            };
        }

        void InitializeCommandProcessors()
        {
            var joinProcessor = new JoinCommandProcessor();
            joinProcessor.Joined += (channel, username) =>
                Dispatcher.Invoke(() => JoinedRoom?.Invoke(channel, username));
            joinProcessor.Joined += (channel, username) => {
                // Bot successfully connected to the channel.
                string lowerName = Connection.Username.ToLowerInvariant();
                if (channel == Connection.Channel && username == lowerName)
                    Dispatcher.Invoke(() => Connected?.Invoke());
            };

            var messageProcessor = new MessageCommandProcessor();
            messageProcessor.MessageReceived += message =>
                Dispatcher.Invoke(() => MessageReceived?.Invoke(message));

            var userStateProcessor = new UserStateCommandProcessor();
            userStateProcessor.AcquireUserState += user =>
                Dispatcher.Invoke(() => AcquireUserState?.Invoke(user));

            // Attach processors.
            commandProcessors = new Dictionary<string, ITwitchCommandProcessor>();
            commandProcessors.Add("JOIN", joinProcessor);
            commandProcessors.Add("PRIVMSG", messageProcessor);
            commandProcessors.Add("USERSTATE", userStateProcessor);
            commandProcessors.Add("PING", new PingCommandProcessor());
        }

        public void Close()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        void IDisposable.Dispose()
        {
            Close();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (Connection != null)
                {
                    Connection.Close();
                    Connection = null;
                }
            }
        }

        public void Start()
        {
            if (runningTask != null)
                return;
            runningTask = Task.Run(() => Run());
        }

        async void Run()
        {
            var messageParser = new TwitchMessageParser();
            int retries = 0;

            while (Connection != null)
            {
                try
                {
                    string data = Connection.Read();
                    if (string.IsNullOrEmpty(data))
                    {
                        // Failed to read, try again.
                        if (retries >= 3)
                            break;
                        retries += 1;
                        await Task.Delay(1000);
                    }
                    else retries = 0;

                    var message = messageParser.Parse(data);
                    if (message == null) continue;

                    // Completely ignore certain messages.
                    if (ignoredCommands.Contains(message.Command))
                        continue;

                    // Process specified commands.
                    ITwitchCommandProcessor commandProcessor;
                    if (commandProcessors.TryGetValue(message.Command, out commandProcessor))
                        commandProcessor.Process(Connection, message);
                    // Raise unhandled messages to UI thread.
                    else Dispatcher.Invoke(() => UnhandledMessage?.Invoke(message));
                }
                catch (IOException)
                {
                    continue;
                }
            }

            try
            {
                // Raise disconnected event.
                Dispatcher.Invoke(() => Disconnected?.Invoke());
            }
            catch (TaskCanceledException) {}
        }
    }
}

// This file is a part of DiabloSpeech and is under the MIT license.
// See the LICENSE file at the root of the project for more information.
using DiabloSpeech.Core;
using DiabloSpeech.Core.Chat;
using DiabloSpeech.Core.Twitch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace DiabloSpeech
{
    /// <summary>
    /// Interaction logic for ChatBotWindow.xaml
    /// </summary>
    public partial class ChatBotWindow : Window
    {
        TwitchClient client;

        public int MaxLogMessageCount { get; } = 100;
        public int CommandTimeoutSeconds { get; } = 10;
        DateTime nextCommandTime = DateTime.Now;
        TwitchUser clientUser = null;
        List<Run> uncoloredNames = new List<Run>();
        Dictionary<string, IChatCommand> chatCommands;

        public ChatBotWindow(ITwitchChannelConnection connection)
        {
            if (connection == null)
                throw new ArgumentNullException(nameof(connection));

            InitializeComponent();
            InitializeTwitchClient(connection);
            InitializeChatCommands();

            // Clear out text in message log.
            messageLog.Document.Blocks.Clear();

            // Disable messaging until we joined a room.
            MessagingEnabled(false);
        }

        void MessagingEnabled(bool enabled)
        {
            sendMessageButton.IsEnabled = enabled;
            messageBox.IsEnabled = enabled;
        }

        void InitializeTwitchClient(ITwitchChannelConnection connection)
        {
            client = new TwitchClient(connection);
            client.Connected += () => {
                var inlines = new List<Inline>();
                inlines.Add(new Bold(new Run($"Successfully connected to channel {connection.Channel}.")));
                LogMessage(inlines);

                client.Connection.Send("/me > started.");

                MessagingEnabled(true);
                client.MessageReceived += client_MessageReceived;
                client.AcquireUserState += client_StateAcquired;
#if DEBUG
                client.UnhandledMessage += message => {
                    Console.WriteLine("Unhandled message: " + message.Raw);
                };
#endif
            };

            client.Disconnected += () => {
                var inlines = new List<Inline>();
                inlines.Add(new Bold(new Run($"Lost connection to channel {connection.Channel}.")));
                LogMessage(inlines);

                MessagingEnabled(false);
            };

            // Finally start the client task.
            client.Start();
        }

        void InitializeChatCommands()
        {
            chatCommands = new Dictionary<string, IChatCommand>();

            var commandMappings = ChatCommandFactory.BuildFromReflection();
            foreach (var commandInfo in commandMappings)
            {
                foreach (var alias in commandInfo.Aliases)
                {
                    if (chatCommands.ContainsKey(alias))
                    {
                        MessageBox.Show($"Chat alias {alias} is colliding for {commandInfo.Name}!\nThe alias will be ignored.",
                            "Chat Command Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                        continue;
                    }

                    chatCommands[alias] = commandInfo.Instance;
                }
            }
        }

        void client_StateAcquired(TwitchUser user)
        {
            clientUser = user;

            if (uncoloredNames.Count > 0)
            {
                var brush = new SolidColorBrush(clientUser.Color);
                foreach (var run in uncoloredNames)
                {
                    run.Text = clientUser.Name;
                    run.Foreground = brush;
                }
                uncoloredNames.Clear();
            }
        }

        void client_MessageReceived(TwitchChatMessage message)
        {
            // Colorize the message.
            Brush userColorBrush = new SolidColorBrush(message.User.Color);

            // Print message to log.
            var inlines = new List<Inline>();
            inlines.Add(new Bold(new Run(message.User.Name) { Foreground = userColorBrush }));
            if (message.Type == TwitchChatMessageType.Normal)
                inlines.Add(new Run(": " + message.Text));
            else if (message.Type == TwitchChatMessageType.Self)
                inlines.Add(new Run(" " + message.Text) { Foreground = userColorBrush });
            LogMessage(inlines);

            // Handle valid commands.
            HandleCommand(message);
        }

        void HandleCommand(TwitchChatMessage message)
        {
            string text = message.Text.Trim();
            if (string.IsNullOrEmpty(text)) return;
            if (!text.StartsWith("!")) return;

            // Get command name.
            string command = "";
            int position = 1;
            int nextSpace = text.IndexOf(' ', position);
            if (nextSpace < 0)
                command = text.Substring(position);
            else command = text.Substring(position, nextSpace - position);
            if (string.IsNullOrEmpty(command)) return;
            command = command.ToLowerInvariant();

            // Get command arguments.
            List<string> arguments = new List<string>();
            if (nextSpace >= 0)
            {
                string argumentsText = text.Substring(nextSpace).Trim();
                if (!string.IsNullOrEmpty(argumentsText))
                {
                    arguments = argumentsText.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                }
            }

            ChatCommandData commandData = new ChatCommandData(command, arguments);

            IChatCommand chatCommand;
            if (chatCommands.TryGetValue(command, out chatCommand))
            {
                // Found the chat command, all valid command have a cooldown.
                if (DateTime.Now < nextCommandTime) return;
                nextCommandTime = DateTime.Now + TimeSpan.FromSeconds(CommandTimeoutSeconds);

                // Do invidual chat command handling.
                var chatWriter = new TwitchChatWriter(client.Connection);
                chatCommand.Process(chatWriter, commandData);
            }
        }

        void LogMessage(string message)
        {
            LogMessage(new[] { new Run(message) });
        }

        void LogMessage(IEnumerable<Inline> inlines)
        {
            string timestamp = DateTime.Now.ToString("[HH:mm:ss] ");
            FlowDocument document = messageLog.Document;
            Paragraph paragraph = new Paragraph();
            paragraph.Inlines.Add(new Run(timestamp) { Foreground = Brushes.SlateGray });
            paragraph.Inlines.AddRange(inlines);

            document.Blocks.Add(paragraph);

            while (document.Blocks.Count > MaxLogMessageCount)
            {
                document.Blocks.Remove(document.Blocks.FirstBlock);
            }

            messageLog.ScrollToEnd();
        }

        void sendMessageButton_Click(object sender, RoutedEventArgs e)
        {
            SendCurrentMessage();
        }

        void SendBotMessage(string message)
        {
            // Send message to twitch chat.
            client.Connection.Send(message);

            // Colorize name if possible.
            Run nameRun = new Run(client.Connection.Username);
            if (clientUser == null)
            {
                uncoloredNames.Add(nameRun);
            }
            else
            {
                nameRun.Text = clientUser.Name;
                nameRun.Foreground = new SolidColorBrush(clientUser.Color);
            }

            // Show message locally.
            var inlines = new List<Inline>();
            inlines.Add(new Bold(nameRun));
            inlines.Add(new Run(": " + message));
            LogMessage(inlines);
        }

        void SendBotMessage(string format, params object[] arguments)
        {
            SendBotMessage(string.Format(format, arguments));
        }

        void SendCurrentMessage()
        {
            string message = (messageBox.Text ?? string.Empty).Trim();
            messageBox.Text = string.Empty;

            // Nothing to send.
            if (message.Length == 0)
                return;

            SendBotMessage(message);

            // Refocus the message box after sending.
            messageBox.Focus();
        }

        void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (client != null)
            {
                client.Connection.Send("/me > terminated.");

                client.Close();
                client = null;
            }
        }

        void messageBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Return)
            {
                e.Handled = true;
                SendCurrentMessage();
            }
        }
    }
}

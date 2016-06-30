using DiabloSpeech.Business.Twitch;

namespace tests.Twitch.Mocks
{
    class TwitchConnectionMock : ITwitchChannelConnection
    {
        public string Channel { get; }
        public string Username { get; }

        public TwitchConnectionMock(string username, string channel)
        {
            Username = username;
            Channel = channel;
        }

        public void Close()
        {
        }

        public void Command(string command)
        {
        }

        public void Command(string format, params object[] args)
        {
        }

        public void Dispose()
        {
        }

        public void Flush()
        {
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

// This file is a part of DiabloSpeech and is under the MIT license.
// See the LICENSE file at the root of the project for more information.
namespace DiabloSpeech.Core.Twitch.Processors
{
    public class PingCommandProcessor : ITwitchCommandProcessor
    {
        public void Process(ITwitchChannelConnection connection, TwitchMessageData data)
        {
            if (connection == null)
                throw new System.ArgumentNullException(nameof(connection));

            connection.Command("PONG");
            connection.Flush();
        }
    }
}

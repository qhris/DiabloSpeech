// This file is a part of DiabloSpeech and is under the MIT license.
// See the LICENSE file at the root of the project for more information.
namespace DiabloSpeech.Business.Twitch.Processors
{
    public class PingCommandProcessor : ITwitchCommandProcessor
    {
        public void Process(ITwitchChannelConnection connection, TwitchMessageData data)
        {
            connection.Command("PONG");
            connection.Flush();
        }
    }
}

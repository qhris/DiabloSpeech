// This file is a part of DiabloSpeech and is under the MIT license.
// See the LICENSE file at the root of the project for more information.
namespace DiabloSpeech.Core.Twitch
{
    public interface ITwitchCommandProcessor
    {
        void Process(ITwitchChannelConnection connection, TwitchMessageData data);
    }
}

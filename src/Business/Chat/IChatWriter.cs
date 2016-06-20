// This file is a part of DiabloSpeech and is under the MIT license.
// See the LICENSE file at the root of the project for more information.
namespace DiabloSpeech.Business.Chat
{
    public interface IChatWriter
    {
        void SendMessage(string message);
        void SendMessage(string format, params string[] args);
    }
}

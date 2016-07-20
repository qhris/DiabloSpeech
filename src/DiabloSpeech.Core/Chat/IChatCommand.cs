// This file is a part of DiabloSpeech and is under the MIT license.
// See the LICENSE file at the root of the project for more information.
using System.Threading.Tasks;

namespace DiabloSpeech.Core.Chat
{
    public interface IChatCommand
    {
        Task Process(IChatWriter chat, ChatCommandData data);
    }
}

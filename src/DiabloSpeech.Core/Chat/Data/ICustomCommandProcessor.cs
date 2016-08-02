// This file is a part of DiabloSpeech and is under the MIT license.
// See the LICENSE file at the root of the project for more information.
using DiabloSpeech.Core.Chat.Data;

namespace DiabloSpeech.Core.Chat.Commands
{
    public interface ICustomCommandProcessor
    {
        CustomCommandCollection CommandCollection { get; set; }
    }
}

// This file is a part of DiabloSpeech and is under the MIT license.
// See the LICENSE file at the root of the project for more information.
using System;
using System.Collections.Generic;

namespace DiabloSpeech.Core.Chat
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    class CommandAliasAttribute : Attribute
    {
        public IReadOnlyList<string> Aliases { get; }

        public CommandAliasAttribute(params string[] aliases)
        {
            Aliases = aliases;
        }
    }
}

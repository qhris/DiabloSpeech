// This file is a part of DiabloSpeech and is under the MIT license.
// See the LICENSE file at the root of the project for more information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DiabloSpeech.Business.Chat
{
    public static class ChatCommandFactory
    {
        public static IEnumerable<ChatCommandInfo> BuildFromReflection()
        {
            // Get chat command aliases.
            Func<Type, IReadOnlyList<string>> getAliases = type => {
                var aliasType = typeof(CommandAliasAttribute);
                var aliasData = Attribute.GetCustomAttribute(type, aliasType) as CommandAliasAttribute;
                if (aliasData == null) return new string[] { };
                return aliasData.Aliases;
            };

            return from type in Assembly.GetExecutingAssembly().GetTypes()
                   // Must implement IChatCommand interface.
                   where typeof(IChatCommand).IsAssignableFrom(type)
                   // Type/command must have a parameterless constructor.
                   where type.GetConstructor(Type.EmptyTypes) != null
                   // Retrieve list of aliases (if possible).
                   let aliases = getAliases(type)
                   let instance = (IChatCommand)Activator.CreateInstance(type)
                   select new ChatCommandInfo(type.Name.ToLowerInvariant(), instance, aliases);
        }
    }
}

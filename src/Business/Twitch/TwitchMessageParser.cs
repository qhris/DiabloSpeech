// This file is a part of DiabloSpeech and is under the MIT license.
// See the LICENSE file at the root of the project for more information.
using System;

namespace DiabloSpeech.Business.Twitch
{
    public class TwitchMessageParser
    {
        /// <summary>
        /// Parse a Twitch IRC message.
        /// </summary>
        /// <returns>The successfully parsed message or null.</returns>
        public TwitchMessageData Parse(string data)
        {
            if (string.IsNullOrEmpty(data))
                return null;

            // Make sure there is actual data to be parsed.
            string buffer = data.TrimStart();
            if (buffer.Length == 0)
                return null;

            // Add null terminator sentinel.
            buffer = buffer + '\0';

            var message = new TwitchMessageData();
            message.Raw = data;
            int position = 0;
            int nextSpace = 0;

            Func<int, int> skipWhitespace = p => {
                while (char.IsWhiteSpace(buffer[p])) p++;
                return p;
            };

            // Check for message tags (IRCv3.2).
            if (buffer[position] == '@')
            {
                // Advance behind the '@'.
                position += 1;

                nextSpace = buffer.IndexOf(' ', position);
                if (nextSpace < 0) return null;

                string[] tagData = buffer.Substring(position, nextSpace - position).Split(';');
                foreach (string tag in tagData)
                {
                    if (tag.IndexOf('=') < 0)
                    {
                        message.Tags[tag] = "true";
                    }
                    else
                    {
                        string[] tagParts = tag.Split(new[] { '=' }, 2);
                        if (string.IsNullOrEmpty(tagParts[0]))
                            continue;

                        message.Tags[tagParts[0]] = tagParts[1];
                    }
                }

                position = nextSpace + 1;
            }

            // Skip whitespace.
            position = skipWhitespace(position);

            // Read prefix if it exists.
            // <message> ::= [':' <prefix> <SPACE>] <command> <params> <crlf>
            // <prefix>  ::= <servername> | <nick> ['!' <user>] ['@' <host>]
            if (buffer[position] == ':')
            {
                // Advance behind the ':'.
                position += 1;

                // Find end of prefix.
                nextSpace = buffer.IndexOf(' ', position);
                if (nextSpace < 0) return null;

                message.Prefix = buffer.Substring(position, nextSpace - position);
                position = skipWhitespace(nextSpace);
            }

            // Read command.
            nextSpace = buffer.IndexOf(' ', position);
            if (nextSpace < 0)
            {
                // Extract command without sentinel.
                int length = buffer.Length - position - 1;
                if (length == 0) return null;

                message.Command = buffer.Substring(position, length);
                return message;
            }

            // Extract command.
            message.Command = buffer.Substring(position, nextSpace - position);
            position = skipWhitespace(nextSpace);

            // Parse command arguments.
            while (position < buffer.Length)
            {
                // Trailing argument.
                if (buffer[position] == ':')
                {
                    position += 1;
                    int length = buffer.Length - position - 1;
                    if (length > 0)
                    {
                        message.Params.Add(buffer.Substring(position, length));
                    }
                    break;
                }

                nextSpace = buffer.IndexOf(' ', position);
                // Last parameter, add the rest of the string (excluding sentinel).
                if (nextSpace < 0)
                {
                    int length = buffer.Length - position - 1;
                    if (length > 0)
                    {
                        message.Params.Add(buffer.Substring(position, length));
                    }
                    break;
                }

                message.Params.Add(buffer.Substring(position, nextSpace - position));
                position = skipWhitespace(nextSpace);
            }

            return message;
        }
    }
}

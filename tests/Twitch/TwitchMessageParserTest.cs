// This file is a part of DiabloSpeech and is under the MIT license.
// See the LICENSE file at the root of the project for more information.
using DiabloSpeech.Business.Twitch;
using NUnit.Framework;

namespace Tests.Twitch
{
    public class TwitchMessageParserTest
    {
        TwitchMessageParser parser = new TwitchMessageParser();

        [TestCase(null)]
        [TestCase("")]
        public void ParsingEmptyStringsReturnNull(string data)
        {
            Assert.That(parser.Parse(data), Is.Null);
        }

        [TestCase(" ")]
        [TestCase("\t\r\n")]
        [TestCase("  ")]
        public void ParsingWhitspaceGracefully(string data)
        {
            Assert.That(parser.Parse(data), Is.Null);
        }

        [TestCase("CAP")]
        [TestCase("CAP *")]
        [TestCase("CAP * ACK")]
        public void ParsingRetrievesCorrectCommand(string data)
        {
            var message = parser.Parse(data);
            Assert.That(message.Command, Is.EqualTo("CAP"));
        }

        [TestCase("CAP")]
        [TestCase(":tmi.twitch.tv CAP")]
        [TestCase("@key=value CAP")]
        [TestCase(":tmi.twitch.tv CAP *")]
        public void ParsingParamsNeverNull(string data)
        {
            var message = parser.Parse(data);
            Assert.That(message.Params, Is.Not.Null);
        }

        [TestCase("CAP", 0)]
        [TestCase("CAP * ACK", 2)]
        [TestCase("@key=value CAP", 0)]
        [TestCase("CAP * ACK :trailing argument with lots of spaces \tand\ttabs!", 3)]
        public void ParsingParametersSetsArgLength(string data, int count)
        {
            var message = parser.Parse(data);
            Assert.That(message.Params.Count, Is.EqualTo(count));
        }

        [TestCase("JOIN #channel", new[] { "#channel" })]
        [TestCase("CAP * ACK", new[] { "*", "ACK" })]
        [TestCase("CAP * ACK :some feature", new[] { "*", "ACK", "some feature" })]
        public void ParsingSetsCorrectArguments(string data, string[] arguments)
        {
            var message = parser.Parse(data);
            Assert.That(message.Params.Count, Is.EqualTo(arguments.Length));
            for (int i = 0; i < arguments.Length; ++i)
                Assert.That(message.Params[i], Is.EqualTo(arguments[i]));
        }

        [TestCase("@")]
        [TestCase(" @")]
        [TestCase("@ ")]
        [TestCase("@test")]
        [TestCase("@test\tCAP")]
        [TestCase("@test\nCAP")]
        public void ParsingInvalidTagsReturnsNull(string data)
        {
            Assert.That(parser.Parse(data), Is.Null);
        }

        [TestCase("CAP")]
        [TestCase(":tmi.twitch.tv CAP")]
        [TestCase("@key=value CAP")]
        [TestCase(":tmi.twitch.tv CAP *")]
        public void ParsingTagsNeverNull(string data)
        {
            var message = parser.Parse(data);
            Assert.That(message.Tags, Is.Not.Null);
        }

        [TestCase("@test CAP", 1)]
        [TestCase("@test;test CAP", 1)]
        [TestCase("@test;data CAP", 2)]
        [TestCase("@test=true;data= CAP", 2)]
        [TestCase("@test=true;=  CAP ", 1)]
        public void ParsingTagsSuccessfully(string data, int count)
        {
            var message = parser.Parse(data);
            Assert.That(message.Tags.Count, Is.EqualTo(count));
            Assert.That(message.Command, Is.EqualTo("CAP"));
        }

        [TestCase(":")]
        [TestCase(" :")]
        [TestCase(": ")]
        [TestCase(":data\tCAP")]
        [TestCase(":data\nCAP")]
        public void ParsingInvalidPrefixReturnsNull(string data)
        {
            Assert.That(parser.Parse(data), Is.Null);
        }

        [TestCase(":data CAP", "data")]
        [TestCase(" :data CAP", "data")]
        [TestCase(":data  CAP", "data")]
        [TestCase(":data\t CAP", "data\t")]
        [TestCase(":data\ttest CAP", "data\ttest")]
        [TestCase(":data\r\nvalue CAP", "data\r\nvalue")]
        public void ParsingPrefixSuccessfully(string data, string prefix)
        {
            var message = parser.Parse(data);
            Assert.That(message.Prefix, Is.EqualTo(prefix));
            Assert.That(message.Command, Is.EqualTo("CAP"));
        }

        [TestCase("CAP * ACK")]
        [TestCase(" CAP * ACK")]
        [TestCase("CAP * ACK :trailing data")]
        [TestCase("@key=value CAP * ACK")]
        [TestCase(":tmi.twich.tv CAP * ACK")]
        [TestCase("@key=value :tmi.twich.tv CAP * ACK")]
        public void ParsingSetsRawMessage(string data)
        {
            var message = parser.Parse(data);
            Assert.That(message.Raw, Is.EqualTo(data));
        }
    }
}

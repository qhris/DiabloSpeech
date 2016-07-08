// This file is a part of DiabloSpeech and is under the MIT license.
// See the LICENSE file at the root of the project for more information.
using DiabloSpeech.Business.Twitch;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using Tests.Twitch.Mocks;

namespace Tests.Twitch
{
    public class TwitchChannelConnectionTest
    {
        MemoryStream memoryStream;
        NetworkStreamMock stream;
        TwitchAuthenticationDetails auth;

        [SetUp]
        public void InitializeTest()
        {
            auth = new TwitchAuthenticationDetails()
            {
                Username = "test",
                Password = "pass123",
                Channel = "#test",
            };

            memoryStream = new MemoryStream();
            stream = new NetworkStreamMock(memoryStream);
        }

        [TearDown]
        public void TearDownTest()
        {
            stream.Close();
        }

        IEnumerable<string> ConsumeStream()
        {
            var originalPosition = memoryStream.Position;
            memoryStream.Seek(0, SeekOrigin.Begin);
            var reader = new StreamReader(memoryStream);

            var lines = new List<string>();
            while (true)
            {
                var line = reader.ReadLine();
                if (line == null) break;
                lines.Add(line);
            }

            memoryStream.Position = originalPosition;
            return lines;
        }

        [Test]
        public void TwitchChannelConnectionThrowsWithInvalidNetworkStream()
        {
            Assert.That(() => new TwitchChannelConnection(null, auth), Throws.ArgumentNullException);
            using (var stream = new NetworkStreamMock(null))
            {
                Assert.That(() => new TwitchChannelConnection(stream, auth), Throws.ArgumentNullException);
            }
        }

        [TestCase("test", "pass123", null)]
        [TestCase("test", null, "#test")]
        [TestCase(null, "pass123", "#test")]
        [TestCase("test", "pass123", "")]
        [TestCase("test", "", "#test")]
        [TestCase("", "pass123", "#test")]
        public void TwitchChannelConnectionThrowsWithInvalidAuthenticationFormat(string name, string pass, string channel)
        {
            TwitchAuthenticationDetails invalidAuth = new TwitchAuthenticationDetails()
            {
                Username = name,
                Password = pass,
                Channel = channel
            };

            Assert.That(() => new TwitchChannelConnection(stream, invalidAuth), Throws.ArgumentException);
        }

        [TestCase("test", "pass123", "test")]
        [TestCase("test", "pass123", "#test")]
        [TestCase("test", "pass123", "Test")]
        public void TwitchChannelConnectionThrowsNothingWithValidData(string name, string pass, string channel)
        {
            TwitchAuthenticationDetails validAuth = new TwitchAuthenticationDetails()
            {
                Username = name,
                Password = pass,
                Channel = channel
            };

            Assert.That(() => {
                var c = new TwitchChannelConnection(stream, validAuth);
                c.Close();
            }, Throws.Nothing);
        }

        [TestCase("TeSt", "#test")]
        [TestCase("#TEsT", "#test")]
        public void TwitchChannelConnectionFixesChannelName(string channel, string expectedChannel)
        {
            TwitchAuthenticationDetails validAuth = new TwitchAuthenticationDetails()
            {
                Username = "test",
                Password = "pass123",
                Channel = channel
            };

            using (var connection = new TwitchChannelConnection(stream, validAuth))
                Assert.That(connection.Channel, Is.EqualTo(expectedChannel));
        }

        [Test, Timeout(1000)]
        public void TwitchChannelConnectionSendsAuthDetails()
        {
            using (var connection = new TwitchChannelConnection(stream, auth))
            {
                var lines = ConsumeStream();

                Assert.That(lines, Contains.Item("PASS pass123"));
                Assert.That(lines, Contains.Item("NICK test"));
                Assert.That(lines, Contains.Item("JOIN #test"));
            }
        }

        [Test, Timeout(1000)]
        public void TwitchChannelConnectionRequestsTwitchCaps()
        {
            using (var connection = new TwitchChannelConnection(stream, auth))
            {
                var lines = ConsumeStream();

                Assert.That(lines, Contains.Item("CAP REQ :twitch.tv/membership"));
                Assert.That(lines, Contains.Item("CAP REQ :twitch.tv/commands"));
                Assert.That(lines, Contains.Item("CAP REQ :twitch.tv/tags"));
            }
        }

        [Test, Timeout(1000)]
        public void TwitchChannelConnectionSendsMessagesCorrectly()
        {
            using (var connection = new TwitchChannelConnection(stream, auth))
            {
                connection.Send("MESSAGE ONE");
                connection.Send("MESSAGE {0}", "TWO");

                var lines = ConsumeStream();
                Assert.That(lines, Contains.Item("PRIVMSG #test :MESSAGE ONE"));
                Assert.That(lines, Contains.Item("PRIVMSG #test :MESSAGE TWO"));
            }
        }

        [Test, Timeout(1000)]
        public void TwitchChannelConnectionsReadsFromBaseStream()
        {
            using (var connection = new TwitchChannelConnection(stream, auth))
            {
                var writer = new StreamWriter(memoryStream);

                string testMessage = ":test!test@test.host.ext #test :test";
                writer.Write(testMessage + "\r\n");
                writer.Flush();

                // Offset including newline characters...
                long offset = testMessage.Length + 2;
                memoryStream.Seek(-offset, SeekOrigin.Current);

                Assert.That(connection.Read(), Is.EqualTo(testMessage));
            }
        }
    }
}

// This file is a part of DiabloSpeech and is under the MIT license.
// See the LICENSE file at the root of the project for more information.
using DiabloSpeech.Business.Twitch;
using DiabloSpeech.Business.Twitch.Processors;
using NUnit.Framework;
using Tests.Twitch.Mocks;

namespace Tests.Twitch
{
    public class PingCommandProcessorTest
    {
        PingCommandProcessor processor;
        TwitchConnectionMock connection;

        TwitchMessageData MessageData =>
            new TwitchMessageData();

        [SetUp]
        public void InitializeTest()
        {
            processor = new PingCommandProcessor();
            connection = new TwitchConnectionMock("test", "#test");
        }

        [Test]
        public void PingCommandThrowsWithoutConnection()
        {
            Assert.That(() => processor.Process(null, MessageData), Throws.ArgumentNullException);
        }

        [Test]
        public void PingCommandSendPongAndFlushes()
        {
            processor.Process(connection, MessageData);
            Assert.That(connection.FiredCommands.Count, Is.EqualTo(2));
            Assert.That(connection.FiredCommands[0], Is.EqualTo("PONG"));
            Assert.That(connection.FiredCommands[1], Is.EqualTo("__FLUSH"));
        }
    }
}

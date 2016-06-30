using DiabloSpeech.Business.Twitch;
using DiabloSpeech.Business.Twitch.Processors;
using NUnit.Framework;
using tests.Twitch.Mocks;

namespace tests.Twitch
{
    public class JoinCommandProcessorTest
    {
        const string TestPrefix = ":test!test@test.server.extension";

        JoinCommandProcessor processor;
        TwitchConnectionMock connection;

        [SetUp]
        public void InitializeTest()
        {
            processor = new JoinCommandProcessor();
            connection = new TwitchConnectionMock("test", "#test");
        }

        void Process(TwitchMessageData data)
        {
            processor.Process(connection, data);
        }

        TwitchMessageData DataWithPrefix =>
            new TwitchMessageData() { Prefix = TestPrefix };

        [Test]
        public void JoinCommandThrowsWithoutConnection()
        {
            var data = new TwitchMessageData();
            Assert.That(() => processor.Process(null, data), Throws.ArgumentNullException);
        }

        [Test]
        public void JoinCommandThrowsWithoutMessageData()
        {
            Assert.That(() => Process(null), Throws.ArgumentNullException);
        }

        [Test]
        public void JoinCommandDoesNotThrowWithoutParams()
        {
            Assert.That(() => Process(DataWithPrefix), Throws.Nothing);
        }

        [Test]
        public void JoinCommandDoesNotThrowWithoutPrefix()
        {
            var dataWithoutPrefix = new TwitchMessageData();
            dataWithoutPrefix.Params.Add("#test");
            dataWithoutPrefix.Prefix = null;

            Assert.That(() => Process(dataWithoutPrefix), Throws.Nothing);
        }

        [Test]
        public void JoinCommandDoesNotInvokeEventWithoutPrefix()
        {
            bool didInvokeJoin = false;
            processor.Joined += (c, u) =>
                didInvokeJoin = true;

            var dataWithoutPrefix = new TwitchMessageData();
            dataWithoutPrefix.Params.Add("#test");
            dataWithoutPrefix.Prefix = null;

            processor.Process(connection, dataWithoutPrefix);
            Assert.That(didInvokeJoin, Is.False);
        }

        [Test]
        public void JoinCommandInvokesEvent()
        {
            bool didInvokeJoin = false;
            processor.Joined += (c, u) =>
                didInvokeJoin = true;

            processor.Process(connection, DataWithPrefix);
            Assert.That(didInvokeJoin, Is.True);
        }

        [Test]
        public void JoinCommandInvokesEventWithCorrectArguments()
        {
            string channel = null;
            string username = null;
            processor.Joined += (c, u) => {
                channel = c;
                username = u;
            };

            var data = DataWithPrefix;
            data.Params.Add("#test");
            processor.Process(connection, data);
            Assert.That(username, Is.EqualTo("test"));
            Assert.That(channel, Is.EqualTo("#test"));
        }
    }
}

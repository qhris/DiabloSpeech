// This file is a part of DiabloSpeech and is under the MIT license.
// See the LICENSE file at the root of the project for more information.
using DiabloSpeech.Core.Twitch;
using DiabloSpeech.Core.Twitch.Processors;
using NUnit.Framework;
using Tests.Twitch.Mocks;

namespace Tests.Twitch
{
    public class MessageCommandTest
    {
        TwitchConnectionMock connection;
        MessageCommandProcessor processor;
        int eventTriggerCount = 0;
        TwitchChatMessage eventMessage = null;

        [SetUp]
        public void InitializeTest()
        {
            eventMessage = null;
            eventTriggerCount = 0;
            connection = new TwitchConnectionMock("test", "#test");
            processor = new MessageCommandProcessor();
            processor.MessageReceived += message => {
                eventMessage = message;
                eventTriggerCount++;
            };
        }

        [Test]
        public void MessageCommandThrowsWithoutConnection()
        {
            var data = new TwitchMessageData();
            Assert.That(() => processor.Process(null, data), Throws.ArgumentNullException);
        }

        [Test]
        public void MessageCommandThrowsWithoutData()
        {
            Assert.That(() => processor.Process(connection, null), Throws.ArgumentNullException);
        }

        [Test]
        public void MessageCommandDoesNotThrowWithoutPrefix()
        {
            var data = new TwitchMessageData();
            Assert.That(() => processor.Process(connection, data), Throws.Nothing);
        }

        [Test]
        public void MessageCommandDoesNotTriggerEventWithoutMessage()
        {
            var data = new TwitchMessageData();
            data.Params.Add("#test");
            processor.Process(connection, data);
            Assert.That(eventTriggerCount, Is.EqualTo(0));
        }

        [Test]
        public void MessageCommandDoesNotTriggerEventOnOtherChannels()
        {
            var data = new TwitchMessageData();
            data.Params.Add("#invalid");
            data.Params.Add("test message");
            processor.Process(connection, data);
            Assert.That(eventTriggerCount, Is.EqualTo(0));
        }

        [Test]
        public void MessageCommandTriggersEventWithValidData()
        {
            var data = new TwitchMessageData();
            data.Params.Add("#test");
            data.Params.Add("test message");
            processor.Process(connection, data);
            Assert.That(eventTriggerCount, Is.EqualTo(1));
        }

        [TestCase(null, "Unknown", "message 1")]
        [TestCase("www!www@www.twitch.tv", "Www", "message 2")]
        [TestCase("www!www@www.twitch.tv", "Www", "message 3")]
        public void MessageCommandSetsCorrectEventData(string prefix, string name, string message)
        {
            var data = new TwitchMessageData();
            data.Prefix = prefix;
            data.Params.Add("#test");
            data.Params.Add(message);
            processor.Process(connection, data);

            Assert.That(eventMessage.Type, Is.EqualTo(TwitchChatMessageType.Normal));
            Assert.That(eventMessage.User.Name, Is.EqualTo(name));
            Assert.That(eventMessage.Text, Is.EqualTo(message));
        }

        [Test]
        public void MessageCommandReadsDisplayNameTag()
        {
            var data = new TwitchMessageData();
            data.Tags["display-name"] = "TestUser";
            data.Prefix = "www!www@www.twitch.tv";
            data.Params.Add("#test");
            data.Params.Add("test message");
            processor.Process(connection, data);

            Assert.That(eventMessage.User.Name, Is.EqualTo("TestUser"));
        }

        [TestCase("")]
        [TestCase("WWW")]
        [TestCase("#51")]
        public void MessageCommandDoesNotThrowOnInvalidColors(string colorString)
        {
            var data = new TwitchMessageData();
            data.Prefix = "www!www@www.twitch.tv";
            data.Params.Add("#test");
            data.Params.Add("test message");
            processor.Process(connection, data);
            data.Tags["color"] = colorString;

            Assert.That(() => processor.Process(connection, data), Throws.Nothing);
        }

        [Test]
        public void MessageCommandSetsSelfMessageTypeOnSpecialMessageContent()
        {
            var data = new TwitchMessageData();
            data.Prefix = "www!www@www.twitch.tv";
            data.Params.Add("#test");
            data.Params.Add("\u0001ACTION test message\u0001");
            processor.Process(connection, data);

            Assert.That(eventMessage.Type, Is.EqualTo(TwitchChatMessageType.Self));
            Assert.That(eventMessage.Text, Is.EqualTo("test message"));
        }
    }
}

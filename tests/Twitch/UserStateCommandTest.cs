// This file is a part of DiabloSpeech and is under the MIT license.
// See the LICENSE file at the root of the project for more information.
using DiabloSpeech.Core.Twitch;
using DiabloSpeech.Core.Twitch.Processors;
using NUnit.Framework;
using Tests.Twitch.Mocks;

namespace Tests.Twitch
{
    public class UserStateCommandTest
    {
        TwitchConnectionMock connection;
        UserStateCommandProcessor processor;
        int eventTriggerCount = 0;
        TwitchUser eventUser = null;

        [SetUp]
        public void InitializeTest()
        {
            eventUser = null;
            eventTriggerCount = 0;
            connection = new TwitchConnectionMock("test", "#test");
            processor = new UserStateCommandProcessor();
            processor.AcquireUserState += user => {
                eventUser = user;
                eventTriggerCount++;
            };
        }

        [Test]
        public void UserStateCommandThrowsWithoutData()
        {
            Assert.That(() => processor.Process(connection, null), Throws.ArgumentNullException);
        }

        [Test]
        public void UserStateCommandThrowsWithoutConnection()
        {
            var data = new TwitchMessageData();
            Assert.That(() => processor.Process(null, data), Throws.ArgumentNullException);
        }

        [TestCase("")]
        [TestCase("#invalid")]
        public void UserStateCommandDoesNothingWithUnknownChannel(string channelName)
        {
            var data = new TwitchMessageData();
            data.Params.Add(channelName);
            processor.Process(connection, data);
            Assert.That(eventTriggerCount, Is.EqualTo(0));
        }

        [Test]
        public void UserStateCommandTriggersEventOnConnectionChannelIfNotSupplied()
        {
            var data = new TwitchMessageData();
            Assert.That(() => processor.Process(connection, data), Throws.Nothing);
            Assert.That(eventTriggerCount, Is.EqualTo(1));
        }

        [Test]
        public void UserStateCommandTriggerEventWithValidData()
        {
            var data = new TwitchMessageData();
            data.Params.Add("#test");
            processor.Process(connection, data);
            Assert.That(eventTriggerCount, Is.EqualTo(1));
        }

        [Test]
        public void UserStateCommandFallsBackToConnectionSettingsWithoutTag()
        {
            var data = new TwitchMessageData();
            processor.Process(connection, data);
            Assert.That(eventUser.Name, Is.EqualTo(connection.Username));
        }

        [Test]
        public void UserStateCommandSetsDisplayName()
        {
            string name = "Variation";
            var data = new TwitchMessageData();
            data.Tags["display-name"] = name;
            processor.Process(connection, data);
            Assert.That(eventUser.Name, Is.EqualTo(name));
        }

        [TestCase("")]
        [TestCase("wwwwwww")]
        [TestCase("#51")]
        public void UserStateCommandDoesNotThrowOnInvalidColors(string colorString)
        {
            var data = new TwitchMessageData();
            data.Tags["color"] = colorString;
            Assert.That(() => processor.Process(connection, data), Throws.Nothing);
        }
    }
}

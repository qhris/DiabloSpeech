// This file is a part of DiabloSpeech and is under the MIT license.
// See the LICENSE file at the root of the project for more information.
using DiabloSpeech.Business.Chat;
using DiabloSpeech.Business.Chat.Commands;
using NUnit.Framework;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using tests.Twitch.Mocks;

namespace tests.Twitch
{
    public class BreakpointsChatCommandTest
    {
        ChatWriterMock chat;
        BreakpointsChatCommand command;

        [SetUp]
        public void InitializeTest()
        {
            chat = new ChatWriterMock();
            command = new BreakpointsChatCommand();
        }

        [Test]
        public void BreakpointsCommandThrowsWithoutChat()
        {
            var data = new ChatCommandData("breakpoints", new[] { "fcr", "sorceress" });
            Assert.That(() => command.Process(null, data), Throws.ArgumentNullException);
        }

        [Test]
        public void BreakpointsCommandThrowsWithoutData()
        {
            Assert.That(() => command.Process(chat, null), Throws.ArgumentNullException);
        }

        [Test]
        public async Task BreakpointsCommandShowsUsageWithIncorrectParameters()
        {
            var data = new ChatCommandData("breakpoints", new[] { "fcr" });
            await command.Process(chat, data);

            var match = Regex.Match(chat.PreviousMessage, @".*usage.*$", RegexOptions.IgnoreCase);
            Assert.IsTrue(match.Success, "Message was: {0}", chat.PreviousMessage);
        }

        [Test]
        public async Task BreakpointsCommandValidatesType()
        {
            var data = new ChatCommandData("breakpoints", new[] { "www", "sorceress" });
            await command.Process(chat, data);

            var match = Regex.Match(chat.PreviousMessage, @".*invalid.*$", RegexOptions.IgnoreCase);
            Assert.IsTrue(match.Success, "Message was: {0}", chat.PreviousMessage);
        }

        [Test]
        public async Task BreakpointsCommandValidatesClass()
        {
            var data = new ChatCommandData("breakpoints", new[] { "fcr", "wwwwwww" });
            await command.Process(chat, data);

            var match = Regex.Match(chat.PreviousMessage, @".*invalid.*$", RegexOptions.IgnoreCase);
            Assert.IsTrue(match.Success, "Message was: {0}", chat.PreviousMessage);
        }

        [Test]
        public async Task BreakpointsCommandWritesBreakpointsOnSuccess()
        {
            var data = new ChatCommandData("breakpoints", new[] { "fcr", "sorceress" });
            await command.Process(chat, data);

            var match = Regex.Match(chat.PreviousMessage, @".*0 9 20 37 63 105 200.*$");
            Assert.IsTrue(match.Success, "Message was: {0}", chat.PreviousMessage);
        }

        [TestCase("fcr", "amazon")]
        [TestCase("fhr", "amazon")]
        [TestCase("fhr", "druid")]
        [TestCase("fcr", "druid")]
        public async Task BreakpointsCommandWorkWithAllTypes(string @type, string @class)
        {
            var data = new ChatCommandData("breapoints", new[] { @type, @class });
            await command.Process(chat, data);

            var match = Regex.Match(chat.PreviousMessage, @".*(\d+ )+.*$");
            Assert.IsTrue(match.Success, "Message was: {0}", chat.PreviousMessage);
        }
    }
}

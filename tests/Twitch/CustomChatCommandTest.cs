using DiabloSpeech.Core.Chat;
using DiabloSpeech.Core.Chat.Commands;
using DiabloSpeech.Core.Chat.Data;
using NUnit.Framework;
using System.Threading.Tasks;
using Tests.Twitch.Mocks;

namespace Tests.Twitch
{
    public class CustomChatCommandTest
    {
        ChatWriterMock chat;
        CustomChatCommand command;
        CustomCommandCollection collection;

        [SetUp]
        public void InitializeTest()
        {
            chat = new ChatWriterMock();
            command = new CustomChatCommand();
            collection = new CustomCommandCollection();
            command.CommandCollection = collection;
        }

        [Test]
        public void CustomChatCommandThrowsWithoutChat()
        {
            var data = new ChatCommandData("command", new string[] { });
            Assert.That(() => command.Process(null, data), Throws.ArgumentNullException);
        }

        [Test]
        public void CustomChatCommandThrowsWithoutData()
        {
            Assert.That(() => command.Process(chat, null), Throws.ArgumentNullException);
        }

        [Test]
        public async Task CustomChatCommandPrintsErrorWithoutCommandCollection()
        {
            var data = new ChatCommandData("command", new string[] { });
            command.CommandCollection = null;
            await command.Process(chat, data);
            Assert.That(chat.PreviousMessage, Contains.Substring("Error"));
        }

        [Test]
        public async Task CustomChatCommandPrintsUsageWithoutArguments()
        {
            var data = new ChatCommandData("command", new string[] { });
            await command.Process(chat, data);
            Assert.That(chat.PreviousMessage, Contains.Substring("add"));
            Assert.That(chat.PreviousMessage, Contains.Substring("remove"));
            Assert.That(chat.PreviousMessage, Contains.Substring("list"));
        }

        [Test]
        public async Task CustomChatCommandPrintsUsageWithInvalidSubCommand()
        {
            var data = new ChatCommandData("command", new[] { "invalid" });
            await command.Process(chat, data);
            Assert.That(chat.PreviousMessage, Contains.Substring("add"));
            Assert.That(chat.PreviousMessage, Contains.Substring("remove"));
            Assert.That(chat.PreviousMessage, Contains.Substring("list"));
        }

        [Test]
        public async Task CustomChatCommandCanAddCommands()
        {
            var data = new ChatCommandData("command", new[] { "add", "test", "test_message" });
            await command.Process(chat, data);
            Assert.That(chat.PreviousMessage, Contains.Substring("Added"));
            Assert.That(collection.ContainsCommand("test"), Is.True);
            Assert.That(collection.Commands["test"].Text, Is.EqualTo("test_message"));
        }

        [Test]
        public async Task CustomChatCommandCanRemoveCommands()
        {
            collection.AddCommand("test", "sub");
            Assert.That(collection.ContainsCommand("test"), Is.True);
            var data = new ChatCommandData("command", new[] { "remove", "test" });
            await command.Process(chat, data);
            Assert.That(collection.ContainsCommand("test"), Is.False);
        }

        [Test]
        public async Task CustomChatCommandCanAddSubCommands()
        {
            var data = new ChatCommandData("command", new[] { "add", "test", "!sub", "test_message" });
            await command.Process(chat, data);
            Assert.That(collection.ContainsSubCommand("test", "sub"), Is.True);
            Assert.That(collection.Commands["test"].Subcommands["sub"], Is.EqualTo("test_message"));
        }

        [Test]
        public async Task CustomChatCommandCanRemoveSubCommands()
        {
            collection.AddSubCommand("test", "sub", "test_message");
            Assert.That(collection.ContainsSubCommand("test", "sub"), Is.True);
            var data = new ChatCommandData("command", new[] { "remove", "test", "sub" });
            await command.Process(chat, data);
            Assert.That(collection.ContainsSubCommand("test", "sub"), Is.False);
        }

        [Test]
        public async Task CustomChatCommandCanListCommands()
        {
            var data = new ChatCommandData("command", new[] { "list" });
            await command.Process(chat, data);
            Assert.That(chat.PreviousMessage, Contains.Substring("No custom"));
            collection.AddCommand("command_one", "test");
            collection.AddCommand("command_two", "data");
            await command.Process(chat, data);
            Assert.That(chat.PreviousMessage, Contains.Substring("command_one"));
            Assert.That(chat.PreviousMessage, Contains.Substring("command_two"));
        }
    }
}

using DiabloSpeech.Core.Chat.Data;
using NUnit.Framework;

namespace Tests.Chat
{
    public class CustomCommandCollectionTest
    {
        CustomCommandCollection collection;

        [SetUp]
        public void InitializeTest()
        {
            collection = new CustomCommandCollection();
        }

        [Test]
        public void CustomCommandCollectionCanAddCommand()
        {
            collection.AddCommand("test", "data");
            Assert.That(collection.ContainsCommand("test"), Is.True);
            Assert.That(collection.Commands.ContainsKey("test"));
            Assert.That(collection.Commands["test"].Text, Is.EqualTo("data"));
        }

        [Test]
        public void CustomCommandCollectionCanAddSubCommandDirectly()
        {
            collection.AddSubCommand("test", "sub", "data");
            Assert.That(collection.ContainsCommand("test"), Is.True);
            Assert.That(collection.ContainsSubCommand("test", "sub"), Is.True);
            Assert.That(collection.Commands["test"].Subcommands["sub"], Is.EqualTo("data"));
        }

        [Test]
        public void CustomCommandCollectionCanChangeCommandTextWithoutRemovingSubCommands()
        {
            collection.AddSubCommand("test", "sub", "data");
            collection.AddCommand("test", "info");
            Assert.That(collection.Commands["test"].Text, Is.EqualTo("info"));
            Assert.That(collection.ContainsSubCommand("test", "sub"));
            Assert.That(collection.Commands["test"].Subcommands["sub"], Is.EqualTo("data"));
        }

        [Test]
        public void CustomCommandCollectionCanRemoveCommands()
        {
            collection.AddSubCommand("test", "sub", "data");
            collection.RemoveCommand("test");
            Assert.That(collection.ContainsCommand("test"), Is.False);
        }
    }
}

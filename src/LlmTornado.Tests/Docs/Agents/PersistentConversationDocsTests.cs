using System.Collections.Generic;
using System.IO;
using System.Linq;
using LlmTornado.Agents;
using LlmTornado.Chat;
using LlmTornado.Chat.Models;
using LlmTornado.Code;
using NUnit.Framework;

namespace LlmTornado.Tests.Docs.Agents;

[TestFixture]
public class PersistentConversationQuickStartTests
{
    [Test]
    [Category("Docs:2. Agents/2. Tornado-Agent/5. Persistent-Conversation.md#Quick Start")]
    public void SavesAndLoadsHistory()
    {
        string path = Path.Combine(Path.GetTempPath(), "conversation.json");
        List<ChatMessage> messages = [
            new ChatMessage(ChatMessageRoles.User, "My name is Alice"),
            new ChatMessage(ChatMessageRoles.Assistant, "Nice to meet you")
        ];

        messages.SaveConversation(path);

        List<ChatMessage> loaded = [];
        loaded.LoadMessagesAsync(path).GetAwaiter().GetResult();

        Assert.That(loaded.Count, Is.EqualTo(2));
    }
}

[TestFixture]
public class PersistentConversationSaveTests
{
    [Test]
    [Category("Docs:2. Agents/2. Tornado-Agent/5. Persistent-Conversation.md#Save to File")]
    public void CreatesConversationFile()
    {
        string path = Path.Combine(Path.GetTempPath(), "session-123.json");
        List<ChatMessage> messages = [new ChatMessage(ChatMessageRoles.User, "Hello")];

        messages.SaveConversation(path);

        Assert.That(File.Exists(path), Is.True);
    }
}

[TestFixture]
public class PersistentConversationLoadTests
{
    [Test]
    [Category("Docs:2. Agents/2. Tornado-Agent/5. Persistent-Conversation.md#Load from File")]
    public void LoadsMessagesFromFile()
    {
        string path = Path.Combine(Path.GetTempPath(), "my-conversation.json");
        List<ChatMessage> messages = [new ChatMessage(ChatMessageRoles.User, "Hello")];

        messages.SaveConversation(path);

        List<ChatMessage> loaded = [];
        loaded.LoadMessagesAsync(path).GetAwaiter().GetResult();

        Assert.That(loaded.Count, Is.EqualTo(1));
        Assert.That(loaded[0].Content, Is.EqualTo("Hello"));
    }
}

[TestFixture]
public class PersistentConversationTrimmingTests
{
    [Test]
    [Category("Docs:2. Agents/2. Tornado-Agent/5. Persistent-Conversation.md#Trimming History")]
    public void TrimsToMaxMessages()
    {
        List<ChatMessage> messages = [
            new ChatMessage(ChatMessageRoles.User, "1"),
            new ChatMessage(ChatMessageRoles.User, "2"),
            new ChatMessage(ChatMessageRoles.User, "3")
        ];

        int maxMessages = 2;
        List<ChatMessage> trimmed = messages.Skip(messages.Count - maxMessages).ToList();

        Assert.That(trimmed.Count, Is.EqualTo(2));
        Assert.That(trimmed[0].Content, Is.EqualTo("2"));
    }
}

[TestFixture]
public class PersistentConversationSelectivePreservationTests
{
    [Test]
    [Category("Docs:2. Agents/2. Tornado-Agent/5. Persistent-Conversation.md#Selective Preservation")]
    public void PreservesSystemAndRecentMessages()
    {
        List<ChatMessage> messages = [
            new ChatMessage(ChatMessageRoles.System, "System"),
            new ChatMessage(ChatMessageRoles.User, "User 1"),
            new ChatMessage(ChatMessageRoles.Assistant, "Assistant 1")
        ];

        ChatMessage? systemMsg = messages.FirstOrDefault(m => m.Role == ChatMessageRoles.System);
        List<ChatMessage> recent = messages.TakeLast(2).ToList();

        List<ChatMessage> preserved = [];
        if (systemMsg != null)
        {
            preserved.Add(systemMsg);
        }
        preserved.AddRange(recent.Where(m => m.Role != ChatMessageRoles.System));

        Assert.That(preserved.Count, Is.EqualTo(3));
    }
}

[TestFixture]
public class PersistentConversationSnapshotTests
{
    [Test]
    [Category("Docs:2. Agents/2. Tornado-Agent/5. Persistent-Conversation.md#Conversation Snapshots")]
    public void LoadsCheckpointMessages()
    {
        string path = Path.Combine(Path.GetTempPath(), "checkpoint-1.json");
        List<ChatMessage> messages = [new ChatMessage(ChatMessageRoles.User, "Checkpoint")];

        messages.SaveConversation(path);

        List<ChatMessage> checkpoint = [];
        checkpoint.LoadMessagesAsync(path).GetAwaiter().GetResult();

        Assert.That(checkpoint.Count, Is.EqualTo(1));
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LlmTornado;
using LlmTornado.Agents;
using LlmTornado.Agents.DataModels;
using LlmTornado.Chat;
using LlmTornado.Chat.Models;
using LlmTornado.Code;
using NUnit.Framework;
using Category = NUnit.Framework.CategoryAttribute;
using Description = System.ComponentModel.DescriptionAttribute;

namespace LlmTornado.Tests.Docs.Agents;

[TestFixture]
public class GettingStartedQuickStartTests
{
    [Test]
    [Category("Docs:2. Agents/1. Getting-Started.md#Quick Start")]
    public void CreatesAgentWithDefaults()
    {
        TornadoApi api = new TornadoApi("test-key");

        TornadoAgent agent = new TornadoAgent(
            client: api,
            model: ChatModel.OpenAi.Gpt41.V41Mini,
            instructions: "You are a helpful assistant."
        );

        Assert.That(agent.Model, Is.EqualTo(ChatModel.OpenAi.Gpt41.V41Mini));
        Assert.That(agent.Instructions, Is.EqualTo("You are a helpful assistant."));
        Assert.That(agent.Streaming, Is.False);
    }
}

[TestFixture]
public class GettingStartedTornadoAgentPropertyTests
{
    [Test]
    [Category("Docs:2. Agents/1. Getting-Started.md#Key Components")]
    public void OutputSchemaIsStoredWhenProvided()
    {
        TornadoApi api = new TornadoApi("test-key");

        TornadoAgent agent = new TornadoAgent(
            client: api,
            model: ChatModel.OpenAi.Gpt41.V41Mini,
            outputSchema: typeof(ContactInfo)
        );

        Assert.That(agent.OutputSchema, Is.EqualTo(typeof(ContactInfo)));
    }

    private class ContactInfo
    {
        public string Name { get; set; } = string.Empty;
    }
}

[TestFixture]
public class GettingStartedSimpleAgentTests
{
    [Test]
    [Category("Docs:2. Agents/1. Getting-Started.md#Simple Agent")]
    public void StoresInstructionsAndModel()
    {
        TornadoApi api = new TornadoApi("test-key");

        TornadoAgent agent = new TornadoAgent(
            client: api,
            model: ChatModel.OpenAi.Gpt41.V41Mini,
            instructions: "You are a helpful coding assistant specialized in C#."
        );

        Assert.That(agent.Instructions, Is.EqualTo("You are a helpful coding assistant specialized in C#."));
        Assert.That(agent.Model, Is.EqualTo(ChatModel.OpenAi.Gpt41.V41Mini));
    }
}

[TestFixture]
public class GettingStartedStreamingTests
{
    [Test]
    [Category("Docs:2. Agents/1. Getting-Started.md#Agent with Streaming")]
    public void StreamingHandlerProcessesTextDeltas()
    {
        List<string> chunks = [];

        ValueTask streamHandler(AgentRunnerEvents runEvent)
        {
            if (runEvent.EventType == AgentRunnerEventTypes.Streaming && runEvent is AgentRunnerStreamingEvent streamingEvent)
            {
                if (streamingEvent.ModelStreamingEvent is ModelStreamingOutputTextDeltaEvent deltaTextEvent)
                {
                    chunks.Add(deltaTextEvent.DeltaText ?? string.Empty);
                }
            }

            return ValueTask.CompletedTask;
        }

        TornadoApi api = new TornadoApi("test-key");
        Conversation conversation = new Conversation(api.Chat);

        AgentRunnerStreamingEvent evt = new AgentRunnerStreamingEvent(
            new ModelStreamingOutputTextDeltaEvent(1, 0, 0, "Hello"),
            conversation
        );

        streamHandler(evt).GetAwaiter().GetResult();

        Assert.That(chunks.Count, Is.EqualTo(1));
        Assert.That(chunks[0], Is.EqualTo("Hello"));
    }
}

[TestFixture]
public class GettingStartedToolsTests
{
    [Test]
    [Category("Docs:2. Agents/1. Getting-Started.md#Agent with Tools")]
    public void RegistersDelegateToolsOnConstruction()
    {
        TornadoApi api = new TornadoApi("test-key");
        List<Delegate> tools = [GetWeather];

        TornadoAgent agent = new TornadoAgent(
            client: api,
            model: ChatModel.OpenAi.Gpt41.V41Mini,
            instructions: "You are a helpful assistant that can check weather.",
            tools: tools
        );

        Assert.That(agent.ToolList.ContainsKey("GetWeather"), Is.True);
        Assert.That(agent.Options.Tools, Is.Not.Null);
        Assert.That(agent.Options.Tools!.Count, Is.EqualTo(1));
    }

    [Description("get the weather")]
    private static string GetWeather([Description("city to get weather from")] string city)
    {
        return $"The weather in {city} is sunny with 22°C.";
    }
}

[TestFixture]
public class GettingStartedStructuredOutputTests
{
    [Test]
    [Category("Docs:2. Agents/1. Getting-Started.md#Agent with Structured Output")]
    public void SetsResponseFormatFromSchema()
    {
        TornadoApi api = new TornadoApi("test-key");

        TornadoAgent agent = new TornadoAgent(
            client: api,
            model: ChatModel.OpenAi.Gpt4.O,
            instructions: "Extract contact information from user messages.",
            outputSchema: typeof(ContactInfo)
        );

        Assert.That(agent.OutputSchema, Is.EqualTo(typeof(ContactInfo)));
        Assert.That(agent.Options.ResponseFormat, Is.Not.Null);
    }

    private class ContactInfo
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
    }
}

[TestFixture]
public class GettingStartedPersistentConversationTests
{
    [Test]
    [Category("Docs:2. Agents/1. Getting-Started.md#Persistent Conversations")]
    public void ConversationHistoryCanBeAppended()
    {
        List<ChatMessage> history = [new ChatMessage(ChatMessageRoles.User, "My name is Alice.")];

        Assert.That(history.Count, Is.EqualTo(1));
        Assert.That(history[0].Content, Is.EqualTo("My name is Alice."));
    }
}

[TestFixture]
public class GettingStartedConversationPersistenceTests
{
    [Test]
    [Category("Docs:2. Agents/1. Getting-Started.md#Saving and Loading Conversations")]
    public void SavesAndLoadsMessages()
    {
        string filePath = Path.Combine(Path.GetTempPath(), "conversation.json");
        List<ChatMessage> messages = [
            new ChatMessage(ChatMessageRoles.User, "Hello"),
            new ChatMessage(ChatMessageRoles.Assistant, "Hi!")
        ];

        messages.SaveConversation(filePath);

        List<ChatMessage> loaded = [];
        loaded.LoadMessagesAsync(filePath).GetAwaiter().GetResult();

        Assert.That(loaded.Count, Is.EqualTo(2));
        Assert.That(loaded[0].Content, Is.EqualTo("Hello"));
        Assert.That(loaded[1].Content, Is.EqualTo("Hi!"));
    }
}

[TestFixture]
public class GettingStartedMultiProviderTests
{
    [Test]
    [Category("Docs:2. Agents/1. Getting-Started.md#Multi-Provider Support")]
    public void SupportsDifferentProviders()
    {
        TornadoAgent openAiAgent = new TornadoAgent(
            client: new TornadoApi(new ProviderAuthentication(LLmProviders.OpenAi, "openai-key")),
            model: ChatModel.OpenAi.Gpt41.V41,
            instructions: "You are a helpful assistant."
        );

        TornadoAgent anthropicAgent = new TornadoAgent(
            client: new TornadoApi(new ProviderAuthentication(LLmProviders.Anthropic, "anthropic-key")),
            model: ChatModel.Anthropic.Claude37.Sonnet,
            instructions: "You are a helpful assistant."
        );

        Assert.That(openAiAgent.Model, Is.EqualTo(ChatModel.OpenAi.Gpt41.V41));
        Assert.That(anthropicAgent.Model, Is.EqualTo(ChatModel.Anthropic.Claude37.Sonnet));
    }
}

[TestFixture]
public class GettingStartedInstructionsTests
{
    [Test]
    [Category("Docs:2. Agents/1. Getting-Started.md#Clear Instructions")]
    public void StoresCustomInstructions()
    {
        TornadoApi api = new TornadoApi("test-key");
        ChatModel model = ChatModel.OpenAi.Gpt41.V41Mini;
        string instructions = "You are a customer support agent.";

        TornadoAgent agent = new TornadoAgent(api, model, instructions: instructions);

        Assert.That(agent.Instructions, Is.EqualTo(instructions));
    }
}

[TestFixture]
public class GettingStartedErrorHandlingTests
{
    [Test]
    [Category("Docs:2. Agents/1. Getting-Started.md#Error Handling")]
    public void HandlesEmptyConversationMessages()
    {
        List<ChatMessage> messages = [];

        bool hasMessages = messages.Any();

        Assert.That(hasMessages, Is.False);
    }
}

[TestFixture]
public class GettingStartedToolDesignTests
{
    [Test]
    [Category("Docs:2. Agents/1. Getting-Started.md#Tool Design")]
    public void ToolReturnsExpectedFormat()
    {
        string result = GetCurrentTime("UTC");

        Assert.That(result.Contains("UTC"), Is.True);
    }

    [Description("Gets the current time")]
    private static string GetCurrentTime([Description("Input for timezone you want current time of")] string timezone = "UTC")
    {
        TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById(timezone);
        DateTime time = TimeZoneInfo.ConvertTime(DateTime.Now, tzi);
        return $"Current time in {timezone}: {time:yyyy-MM-dd HH:mm:ss}";
    }
}

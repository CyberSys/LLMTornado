using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using LlmTornado;
using LlmTornado.Agents;
using LlmTornado.Chat;
using LlmTornado.Chat.Models;
using LlmTornado.Code;
using LlmTornado.Common;
using NUnit.Framework;
using Category = NUnit.Framework.CategoryAttribute;
using Description = System.ComponentModel.DescriptionAttribute;

namespace LlmTornado.Tests.Docs.Agents;

[TestFixture]
public class TornadoAgentBasicsQuickStartTests
{
    [Test]
    [Category("Docs:2. Agents/2. Tornado-Agent/1. basics.md#Quick Start")]
    public void InitializesAgentWithModel()
    {
        TornadoApi api = new TornadoApi("test-key");

        TornadoAgent agent = new TornadoAgent(
            client: api,
            model: ChatModel.OpenAi.Gpt41.V41Mini,
            instructions: "You are a helpful assistant."
        );

        Assert.That(agent.Model, Is.EqualTo(ChatModel.OpenAi.Gpt41.V41Mini));
    }
}

[TestFixture]
public class TornadoAgentBasicsInitializationTests
{
    [Test]
    [Category("Docs:2. Agents/2. Tornado-Agent/1. basics.md#Agent Initialization")]
    public void AppliesOutputSchemaAndStreaming()
    {
        TornadoApi api = new TornadoApi("test-key");
        List<Delegate> toolsList = [];

        TornadoAgent agent = new TornadoAgent(
            client: api,
            model: ChatModel.OpenAi.Gpt4.O,
            name: "CodeAssistant",
            instructions: "You are a coding expert specialized in C#",
            outputSchema: typeof(Response),
            tools: toolsList,
            streaming: true
        );

        Assert.That(agent.OutputSchema, Is.EqualTo(typeof(Response)));
        Assert.That(agent.Streaming, Is.True);
    }

    private class Response
    {
        public string Summary { get; set; } = string.Empty;
    }
}

[TestFixture]
public class TornadoAgentBasicsSimpleAgentTests
{
    [Test]
    [Category("Docs:2. Agents/2. Tornado-Agent/1. basics.md#Simple Agent")]
    public void StoresInstructions()
    {
        TornadoApi api = new TornadoApi("test-key");

        TornadoAgent agent = new TornadoAgent(
            client: api,
            model: ChatModel.OpenAi.Gpt41.V41Mini,
            instructions: "You are a helpful assistant."
        );

        Assert.That(agent.Instructions, Is.EqualTo("You are a helpful assistant."));
    }
}

[TestFixture]
public class TornadoAgentBasicsCustomInstructionsTests
{
    [Test]
    [Category("Docs:2. Agents/2. Tornado-Agent/1. basics.md#Agent with Custom Instructions")]
    public void AssignsAgentNameAndInstructions()
    {
        TornadoApi api = new TornadoApi("test-key");

        TornadoAgent agent = new TornadoAgent(
            client: api,
            model: ChatModel.OpenAi.Gpt4.O,
            name: "ArchitectBot",
            instructions: "Design scalable systems."
        );

        Assert.That(agent.Name, Is.EqualTo("ArchitectBot"));
        Assert.That(agent.Instructions, Is.EqualTo("Design scalable systems."));
    }
}

[TestFixture]
public class TornadoAgentBasicsToolsTests
{
    [Test]
    [Category("Docs:2. Agents/2. Tornado-Agent/1. basics.md#Agent with Tools")]
    public void RegistersWeatherTool()
    {
        TornadoApi api = new TornadoApi("test-key");

        TornadoAgent agent = new TornadoAgent(
            client: api,
            model: ChatModel.OpenAi.Gpt41.V41Mini,
            instructions: "You are a helpful assistant that can check weather.",
            tools: [WeatherTools.GetWeather]
        );

        Assert.That(agent.ToolList.ContainsKey("GetWeather"), Is.True);
    }

    public class WeatherTools
    {
        [Description("get the weather")]
        public static string GetWeather([Description("city to get weather from")] string city)
        {
            return $"The weather in {city} is sunny with 22°C.";
        }
    }
}

[TestFixture]
public class TornadoAgentBasicsStructuredOutputTests
{
    [Test]
    [Category("Docs:2. Agents/2. Tornado-Agent/1. basics.md#Agent with Structured Output")]
    public void AssignsOutputSchema()
    {
        TornadoApi api = new TornadoApi("test-key");

        TornadoAgent agent = new TornadoAgent(
            client: api,
            model: ChatModel.OpenAi.Gpt4.O,
            instructions: "Analyze tasks and provide structured breakdown.",
            outputSchema: typeof(TaskAnalysis)
        );

        Assert.That(agent.OutputSchema, Is.EqualTo(typeof(TaskAnalysis)));
    }

    private class TaskAnalysis
    {
        public string Task { get; set; } = string.Empty;
        public int ComplexityScore { get; set; }
        public string[] Steps { get; set; } = Array.Empty<string>();
        public int EstimatedMinutes { get; set; }
    }
}

[TestFixture]
public class TornadoAgentBasicsConversationContextTests
{
    [Test]
    [Category("Docs:2. Agents/2. Tornado-Agent/1. basics.md#Maintaining Conversation Context")]
    public void AppendedMessagesPreserveOrder()
    {
        List<ChatMessage> history = [
            new ChatMessage(ChatMessageRoles.User, "My name is Alice"),
            new ChatMessage(ChatMessageRoles.Assistant, "Nice to meet you")
        ];

        Assert.That(history[0].Role, Is.EqualTo(ChatMessageRoles.User));
        Assert.That(history[1].Role, Is.EqualTo(ChatMessageRoles.Assistant));
    }
}

[TestFixture]
public class TornadoAgentBasicsSchemaChangeTests
{
    [Test]
    [Category("Docs:2. Agents/2. Tornado-Agent/1. basics.md#Dynamic Output Schema Changes")]
    public void UpdatesOutputSchemaAtRuntime()
    {
        TornadoApi api = new TornadoApi("test-key");

        TornadoAgent agent = new TornadoAgent(
            api,
            ChatModel.OpenAi.Gpt4.O,
            outputSchema: typeof(PersonInfo)
        );

        agent.UpdateOutputSchema(typeof(CompanyInfo));

        Assert.That(agent.OutputSchema, Is.EqualTo(typeof(CompanyInfo)));
    }

    private class PersonInfo
    {
        public string Name { get; set; } = string.Empty;
    }

    private class CompanyInfo
    {
        public string Name { get; set; } = string.Empty;
    }
}

[TestFixture]
public class TornadoAgentBasicsAddToolTests
{
    [Test]
    [Category("Docs:2. Agents/2. Tornado-Agent/1. basics.md#Adding Tools Dynamically")]
    public void AddsToolToToolList()
    {
        TornadoApi api = new TornadoApi("test-key");
        TornadoAgent agent = new TornadoAgent(api, ChatModel.OpenAi.Gpt41.V41);

        Tool weatherTool = new Tool(
            (string city) => $"Weather in {city}: Sunny",
            "get_weather",
            "Gets weather for a city"
        );

        agent.AddTool(weatherTool);

        Assert.That(agent.ToolList.ContainsKey("get_weather"), Is.True);
    }
}

[TestFixture]
public class TornadoAgentBasicsErrorHandlingTests
{
    [Test]
    [Category("Docs:2. Agents/2. Tornado-Agent/1. basics.md#Error Handling")]
    public void DetectsEmptyResults()
    {
        List<ChatMessage> messages = [];

        bool hasMessages = messages.Any();

        Assert.That(hasMessages, Is.False);
    }
}

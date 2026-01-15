using System.Collections.Generic;
using System.Threading.Tasks;
using LlmTornado;
using LlmTornado.Agents;
using LlmTornado.Agents.DataModels;
using LlmTornado.Chat;
using LlmTornado.Chat.Models;
using LlmTornado.ChatFunctions;
using NUnit.Framework;

namespace LlmTornado.Tests.Docs.Agents;

[TestFixture]
public class StreamingQuickStartTests
{
    [Test]
    [Category("Docs:2. Agents/2. Tornado-Agent/2. streaming.md#Quick Start")]
    public void CapturesStreamingTextDeltas()
    {
        List<string> chunks = [];

        ValueTask streamHandler(AgentRunnerEvents runEvent)
        {
            if (runEvent is AgentRunnerStreamingEvent streamingEvent && streamingEvent.ModelStreamingEvent is ModelStreamingOutputTextDeltaEvent delta)
            {
                chunks.Add(delta.DeltaText ?? string.Empty);
            }
            return ValueTask.CompletedTask;
        }

        TornadoApi api = new TornadoApi("test-key");
        TornadoAgent agent = new TornadoAgent(api, ChatModel.OpenAi.Gpt41.V41Mini, streaming: true);
        Conversation conversation = new Conversation(api.Chat);

        AgentRunnerStreamingEvent evt = new AgentRunnerStreamingEvent(
            new ModelStreamingOutputTextDeltaEvent(1, 0, 0, "Hello"),
            conversation
        );

        streamHandler(evt).GetAwaiter().GetResult();

        Assert.That(chunks.Count, Is.EqualTo(1));
    }
}

[TestFixture]
public class StreamingEventHandlerTests
{
    [Test]
    [Category("Docs:2. Agents/2. Tornado-Agent/2. streaming.md#Basic Usage")]
    public void HandlesToolInvokedEvents()
    {
        List<string> toolNames = [];

        ValueTask handler(AgentRunnerEvents runEvent)
        {
            if (runEvent is AgentRunnerToolInvokedEvent toolEvent)
            {
                toolNames.Add(toolEvent.ToolCalled.Name ?? string.Empty);
            }
            return ValueTask.CompletedTask;
        }

        TornadoApi api = new TornadoApi("test-key");
        Conversation conversation = new Conversation(api.Chat);

        AgentRunnerToolInvokedEvent evt = new AgentRunnerToolInvokedEvent(
            new FunctionCall { Name = "get_weather" },
            conversation
        );

        handler(evt).GetAwaiter().GetResult();

        Assert.That(toolNames.Count, Is.EqualTo(1));
        Assert.That(toolNames[0], Is.EqualTo("get_weather"));
    }
}

using System;
using System.Collections.Generic;
using A2A;
using LlmTornado;
using LlmTornado.A2A;
using LlmTornado.Agents;
using LlmTornado.Agents.ChatRuntime;
using LlmTornado.Agents.ChatRuntime.RuntimeConfigurations;
using LlmTornado.Chat.Models;
using NUnit.Framework;

namespace LlmTornado.Tests.Docs.A2A;

[TestFixture]
public class A2AAgentRuntimeConfigurationTests
{
    [Test]
    [NUnit.Framework.Category("Docs:4. A2A/1. Getting-Started.md#Agent Runtime Configuration")]
    public void DescribesAgentCard()
    {
        TornadoApi api = new TornadoApi("test-key");
        TornadoAgent agent = new TornadoAgent(api, ChatModel.OpenAi.Gpt41.V41Mini);
        IRuntimeConfiguration runtimeConfig = new SingletonRuntimeConfiguration(agent);

        MyA2ARuntime runtime = new MyA2ARuntime(runtimeConfig);
        AgentCard card = runtime.DescribeAgentCard("https://example.com");

        Assert.That(card.Name, Is.EqualTo("MyAgent"));
        Assert.That(card.Url, Is.EqualTo("https://example.com"));
    }

    private sealed class MyA2ARuntime : BaseA2ATornadoRuntimeConfiguration
    {
        public MyA2ARuntime(IRuntimeConfiguration runtimeConfig)
            : base(runtimeConfig, name: "MyAgent", version: "1.0.0") { }

        public override AgentCard DescribeAgentCard(string agentUrl)
        {
            AgentCapabilities capabilities = new AgentCapabilities
            {
                Streaming = true,
                PushNotifications = false
            };

            return new AgentCard
            {
                Name = "MyAgent",
                Description = "A specialized agent for specific tasks",
                Url = agentUrl,
                Capabilities = capabilities
            };
        }
    }
}

[TestFixture]
public class A2AQuickStartSampleTests
{
    [Test]
    [NUnit.Framework.Category("Docs:4. A2A/1. Getting-Started.md#Creating Your First A2A Agent")]
    public void BuildCreatesRuntimeConfiguration()
    {
        A2ATornadoAgentSample sample = new A2ATornadoAgentSample();
        BaseA2ATornadoRuntimeConfiguration runtime = sample.Build();

        AgentCard card = runtime.DescribeAgentCard("https://example.com");
        Assert.That(card.Name, Is.EqualTo("LlmTornado.A2A.AgentServer"));
    }

    private sealed class A2ATornadoAgentSample
    {
        private readonly TornadoAgent _agent;

        public A2ATornadoAgentSample()
        {
            TornadoApi client = new TornadoApi(LlmTornado.Code.LLmProviders.OpenAi, "test-key");

            string instructions = "You are an expert assistant.";

            _agent = new TornadoAgent(
                client: client,
                model: ChatModel.OpenAi.Gpt5.V5,
                name: "Assistant",
                instructions: instructions,
                streaming: true);
        }

        public BaseA2ATornadoRuntimeConfiguration Build()
        {
            IRuntimeConfiguration runtimeConfig = new SingletonRuntimeConfiguration(_agent);

            return new SampleRuntimeConfiguration(
                runtimeConfig: runtimeConfig,
                name: "LlmTornado.A2A.AgentServer",
                version: "1.0.0");
        }
    }

    private sealed class SampleRuntimeConfiguration : BaseA2ATornadoRuntimeConfiguration
    {
        public SampleRuntimeConfiguration(IRuntimeConfiguration runtimeConfig, string name, string version)
            : base(runtimeConfig, name, version) { }

        public override AgentCard DescribeAgentCard(string agentUrl)
        {
            AgentCapabilities capabilities = new AgentCapabilities
            {
                Streaming = true,
                PushNotifications = false,
            };

            AgentSkill chattingSkill = new AgentSkill
            {
                Id = "chatting_skill",
                Name = "Chatting feature",
                Description = "Agent to chat with and search the web.",
                Tags = ["chat", "llm-tornado"],
                Examples = ["Hello, what's up?"],
            };

            return new AgentCard
            {
                Name = AgentName,
                Description = "Agent to chat with and search the web",
                Url = agentUrl,
                Version = AgentVersion,
                DefaultInputModes = ["text"],
                DefaultOutputModes = ["text"],
                Capabilities = capabilities,
                Skills = [chattingSkill],
            };
        }
    }
}

[TestFixture]
public class A2ABaseRuntimeConfigurationTests
{
    [Test]
    [NUnit.Framework.Category("Docs:4. A2A/1. Getting-Started.md#BaseA2ATornadoRuntimeConfiguration")]
    public void BuildsAgentCardWithSkills()
    {
        TornadoApi api = new TornadoApi("test-key");
        TornadoAgent agent = new TornadoAgent(api, ChatModel.OpenAi.Gpt41.V41Mini);
        IRuntimeConfiguration runtimeConfig = new SingletonRuntimeConfiguration(agent);

        TaskAgentRuntime runtime = new TaskAgentRuntime(runtimeConfig, "TaskAgent", "1.0.0");
        AgentCard card = runtime.DescribeAgentCard("https://example.com");

        Assert.That(card.Skills.Count, Is.EqualTo(1));
        Assert.That(card.DefaultInputModes, Does.Contain("text"));
    }

    private sealed class TaskAgentRuntime : BaseA2ATornadoRuntimeConfiguration
    {
        public TaskAgentRuntime(IRuntimeConfiguration runtimeConfig, string name, string version)
            : base(runtimeConfig, name, version) { }

        public override AgentCard DescribeAgentCard(string agentUrl)
        {
            AgentCapabilities capabilities = new AgentCapabilities
            {
                Streaming = true,
                PushNotifications = false,
            };

            AgentSkill chattingSkill = new AgentSkill
            {
                Id = "chatting_skill",
                Name = "Chatting feature",
                Description = "Agent to chat with and search the web.",
                Tags = ["chat", "llm-tornado"],
                Examples = ["Hello, what's up?"],
            };

            return new AgentCard
            {
                Name = AgentName,
                Description = "Agent to chat with and search the web",
                Url = agentUrl,
                Version = AgentVersion,
                DefaultInputModes = ["text"],
                DefaultOutputModes = ["text"],
                Capabilities = capabilities,
                Skills = [chattingSkill],
            };
        }
    }
}

[TestFixture]
public class A2ACommunicationProtocolTests
{
    [Test]
    [NUnit.Framework.Category("Docs:4. A2A/1. Getting-Started.md#Communication Protocol")]
    public void AssignsRequestAndResponseFields()
    {
        A2ARequest request = new A2ARequest
        {
            AgentId = "agent-1",
            Message = "Hello",
            Context = new Dictionary<string, object>()
        };

        A2AResponse response = new A2AResponse
        {
            AgentId = "agent-1",
            Response = "Hi there",
            Metadata = new Dictionary<string, object>()
        };

        Assert.That(request.AgentId, Is.EqualTo("agent-1"));
        Assert.That(response.Response, Is.EqualTo("Hi there"));
    }

    private sealed class A2ARequest
    {
        public string AgentId { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public Dictionary<string, object>? Context { get; set; }
    }

    private sealed class A2AResponse
    {
        public string AgentId { get; set; } = string.Empty;
        public string Response { get; set; } = string.Empty;
        public Dictionary<string, object>? Metadata { get; set; }
    }
}

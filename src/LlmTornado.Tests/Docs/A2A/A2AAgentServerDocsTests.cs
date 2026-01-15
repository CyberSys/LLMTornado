using System.Net.Http;
using System.Text;
using A2A;
using LlmTornado;
using LlmTornado.A2A;
using LlmTornado.Agents;
using LlmTornado.Agents.ChatRuntime;
using LlmTornado.Agents.ChatRuntime.RuntimeConfigurations;
using LlmTornado.Chat.Models;
using Newtonsoft.Json;
using NUnit.Framework;

namespace LlmTornado.Tests.Docs.A2A;

[TestFixture]
public class A2AAgentServerStep1Tests
{
    [Test]
    [NUnit.Framework.Category("Docs:4. A2A/2. Agent-Server.md#Step 1: Create Your Agent")]
    public void BuildCreatesAgentCard()
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
public class A2AAgentServerStep2Tests
{
    [Test]
    [NUnit.Framework.Category("Docs:4. A2A/2. Agent-Server.md#Step 2: Add Agent to Program.cs")]
    public void CreatesRuntimeFromSample()
    {
        BaseA2ATornadoRuntimeConfiguration agentRuntime = new SampleFactory().Build();

        Assert.That(agentRuntime, Is.Not.Null);
    }

    private sealed class SampleFactory
    {
        public BaseA2ATornadoRuntimeConfiguration Build()
        {
            TornadoApi api = new TornadoApi("test-key");
            TornadoAgent agent = new TornadoAgent(api, ChatModel.OpenAi.Gpt41.V41Mini);
            IRuntimeConfiguration runtimeConfig = new SingletonRuntimeConfiguration(agent);

            return new SimpleRuntimeConfiguration(runtimeConfig, "Server", "1.0.0");
        }
    }

    private sealed class SimpleRuntimeConfiguration : BaseA2ATornadoRuntimeConfiguration
    {
        public SimpleRuntimeConfiguration(IRuntimeConfiguration runtimeConfig, string name, string version)
            : base(runtimeConfig, name, version) { }

        public override AgentCard DescribeAgentCard(string agentUrl)
        {
            return new AgentCard
            {
                Name = AgentName,
                Description = "Agent",
                Url = agentUrl,
                Version = AgentVersion
            };
        }
    }
}

[TestFixture]
public class A2AAgentServerHttpClientTests
{
    [Test]
    [NUnit.Framework.Category("Docs:4. A2A/2. Agent-Server.md#Using HTTPClient")]
    public void BuildsJsonContent()
    {
        StringContent content = new StringContent(
            JsonConvert.SerializeObject(new { message = "What's the weather?" }),
            Encoding.UTF8,
            "application/json");

        Assert.That(content.Headers.ContentType?.MediaType, Is.EqualTo("application/json"));
    }
}

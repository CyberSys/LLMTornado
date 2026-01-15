using LlmTornado;
using LlmTornado.Agents;
using LlmTornado.Chat.Models;
using LlmTornado.Responses;
using NUnit.Framework;

namespace LlmTornado.Tests.Docs.Agents;

[TestFixture]
public class ResponseToolsQuickStartTests
{
    [Test]
    [Category("Docs:2. Agents/2. Tornado-Agent/4. Tools/2. Response-Tools.md#Quick Start")]
    public void AttachesResponseTools()
    {
        TornadoApi api = new TornadoApi("test-key");
        TornadoAgent agent = new TornadoAgent(api, ChatModel.OpenAi.Gpt5.V5Mini, name: "assistant");

        agent.ResponseOptions = new ResponseRequest
        {
            Tools = [new ResponseWebSearchTool()]
        };

        Assert.That(agent.ResponseOptions, Is.Not.Null);
        Assert.That(agent.ResponseOptions!.Tools.Count, Is.EqualTo(1));
    }
}

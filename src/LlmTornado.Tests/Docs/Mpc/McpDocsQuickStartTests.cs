using LlmTornado;
using LlmTornado.Agents;
using LlmTornado.Chat.Models;
using LlmTornado.Common;
using LlmTornado.Mcp;
using NUnit.Framework;
using Category = NUnit.Framework.CategoryAttribute;

namespace LlmTornado.Tests.Docs.Mpc;

[TestFixture]
public class McpQuickStartTests
{
    [Test]
    [NUnit.Framework.Category("Docs:3. MPC/MPC.md#Quick Start")]
    public void RegistersMcpToolsOnAgent()
    {
        MCPServer gmailServer = MCPToolkits.Gmail(
            allowedTools: ["read_email", "draft_email", "search_emails"]);

        gmailServer.AllowedTornadoTools.Add(new Tool((string input) => input, "read_email", "Read email"));

        TornadoApi api = new TornadoApi("test-key");
        TornadoAgent agent = new TornadoAgent(
            client: api,
            model: ChatModel.OpenAi.Gpt41.V41Mini,
            instructions: "You are a useful assistant for managing Gmail."
        );

        agent.AddTool(gmailServer.AllowedTornadoTools);

        Assert.That(agent.ToolList.ContainsKey("read_email"), Is.True);
    }
}

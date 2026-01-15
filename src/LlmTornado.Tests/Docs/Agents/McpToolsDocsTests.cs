using System.Collections.Generic;
using LlmTornado;
using LlmTornado.Agents;
using LlmTornado.Chat.Models;
using LlmTornado.Common;
using LlmTornado.Mcp;
using NUnit.Framework;

namespace LlmTornado.Tests.Docs.Agents;

[TestFixture]
public class McpToolsQuickStartTests
{
    [Test]
    [Category("Docs:2. Agents/2. Tornado-Agent/4. Tools/3. MCP-Tools.md#Quick Start")]
    public void RegistersMcpToolsOnAgent()
    {
        MCPServer gmailServer = new MCPServer(
            serverLabel: "gmail",
            command: "npx",
            arguments: new[] { "@gongrzhe/server-gmail-autoauth-mcp" },
            allowedTools: ["read_email", "draft_email", "search_emails"]
        );

        gmailServer.AllowedTornadoTools.Add(new Tool((string input) => input, "read_email", "Read email"));

        TornadoApi api = new TornadoApi("test-key");
        TornadoAgent agent = new TornadoAgent(api, ChatModel.OpenAi.Gpt41.V41Mini);

        agent.AddTool(gmailServer.AllowedTornadoTools);

        Assert.That(agent.ToolList.ContainsKey("read_email"), Is.True);
    }
}

[TestFixture]
public class McpToolsHttpTransportTests
{
    [Test]
    [Category("Docs:2. Agents/2. Tornado-Agent/4. Tools/3. MCP-Tools.md#HTTP Transport Configuration")]
    public void StoresAdditionalHeaders()
    {
        Dictionary<string, string> headers = [];
        headers["Authorization"] = "Bearer token";

        MCPServer server = new MCPServer(
            "github",
            "https://api.githubcopilot.com/mcp",
            additionalConnectionHeaders: headers,
            allowedTools: []
        );

        Assert.That(server.AdditionalConnectionHeaders, Is.Not.Null);
        Assert.That(server.AdditionalConnectionHeaders!.Count, Is.EqualTo(1));
    }
}

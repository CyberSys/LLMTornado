using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using LlmTornado.ChatFunctions;
using LlmTornado.Common;
using LlmTornado.Mcp;
using ModelContextProtocol.Authentication;
using ModelContextProtocol.Client;
using NUnit.Framework;
using Category = NUnit.Framework.CategoryAttribute;

namespace LlmTornado.Tests.Docs.Mpc;

[TestFixture]
public class McpListTornadoToolsAsyncTests
{
    [Test]
    [NUnit.Framework.Category("Docs:3. MPC/MPC.md#ListTornadoToolsAsync")]
    public void ExtensionMethodExists()
    {
        MethodInfo? method = typeof(McpExtensions)
            .GetMethod("ListTornadoToolsAsync", BindingFlags.Public | BindingFlags.Static);

        Assert.That(method, Is.Not.Null);
        Assert.That(method!.ReturnType, Is.EqualTo(typeof(ValueTask<List<Tool>>)));

        ParameterInfo[] parameters = method.GetParameters();
        Assert.That(parameters.Length, Is.EqualTo(1));
        Assert.That(parameters[0].ParameterType, Is.EqualTo(typeof(McpClient)));
    }
}

[TestFixture]
public class McpResolveRemoteTests
{
    [Test]
    [NUnit.Framework.Category("Docs:3. MPC/MPC.md#ResolveRemote")]
    public void ResolveRemoteSetsFailureWhenNoRemoteTool()
    {
        FunctionCall call = new FunctionCall { Name = "remote_tool" };

        call.ResolveRemote().GetAwaiter().GetResult();

        Assert.That(call.Result, Is.Not.Null);
        Assert.That(call.Result!.InvocationSucceeded, Is.False);
    }
}

[TestFixture]
public class McpServerClassTests
{
    [Test]
    [NUnit.Framework.Category("Docs:3. MPC/MPC.md#MCPServer Class")]
    public void ConstructsHttpServerWithHeaders()
    {
        Dictionary<string, string> headers = [];
        headers["Authorization"] = "Bearer token";

        MCPServer server = new MCPServer(
            serverLabel: "github",
            serverUrl: "https://api.githubcopilot.com/mcp",
            allowedTools: ["list_repos"],
            additionalConnectionHeaders: headers,
            oAuthOptions: new ClientOAuthOptions
            {
                RedirectUri = new Uri("http://localhost")
            }
        );

        Assert.That(server.ServerLabel, Is.EqualTo("github"));
        Assert.That(server.AdditionalConnectionHeaders, Is.Not.Null);
        Assert.That(server.AllowedTools, Is.Not.Null);
    }

    [Test]
    [NUnit.Framework.Category("Docs:3. MPC/MPC.md#MCPServer Class")]
    public void ConstructsStdioServer()
    {
        MCPServer server = new MCPServer(
            serverLabel: "gmail",
            command: "npx",
            arguments: new[] { "@gongrzhe/server-gmail-autoauth-mcp" },
            workingDirectory: "",
            environmentVariables: null,
            allowedTools: ["read_email"]
        );

        Assert.That(server.Command, Is.EqualTo("npx"));
        Assert.That(server.AllowedTools, Is.Not.Null);
    }
}

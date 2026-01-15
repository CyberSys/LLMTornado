using System;
using System.Collections.Generic;
using System.ComponentModel;
using LlmTornado;
using LlmTornado.Agents;
using LlmTornado.Chat.Models;
using LlmTornado.Common;
using NUnit.Framework;
using Category = NUnit.Framework.CategoryAttribute;
using Description = System.ComponentModel.DescriptionAttribute;

namespace LlmTornado.Tests.Docs.Agents;

[TestFixture]
public class FunctionToolQuickStartTests
{
    [Test]
    [Category("Docs:2. Agents/2. Tornado-Agent/4. Tools/1. Function-Tools.md#Quick Start")]
    public void RegistersDelegateTools()
    {
        TornadoApi api = new TornadoApi("test-key");

        TornadoAgent agent = new TornadoAgent(
            client: api,
            model: ChatModel.OpenAi.Gpt41.V41,
            instructions: "You are a helpful assistant with weather and calculation tools.",
            tools: [GetWeather, Calculate]
        );

        Assert.That(agent.ToolList.ContainsKey("GetWeatherTool"), Is.True);
        Assert.That(agent.ToolList.ContainsKey("Calculate"), Is.True);
    }

    [Description("get the weather")]
    [ToolName("GetWeatherTool")]
    private static string GetWeather([Description("city to get weather from")] string city)
    {
        return $"Weather in {city}: 22°C, Sunny";
    }

    [Description("perform add Or multiply math Calculation")]
    private static string Calculate(
        [Description("math operation to perform (add or multiply)")]
        string operation,
        double a,
        double b)
    {
        double result = operation switch
        {
            "add" => a + b,
            "multiply" => a * b,
            _ => 0
        };

        return result.ToString();
    }
}

[TestFixture]
public class SimpleFunctionToolTests
{
    [Test]
    [Category("Docs:2. Agents/2. Tornado-Agent/4. Tools/1. Function-Tools.md#Simple Function Tool")]
    public void ConvertsDelegateToTool()
    {
        TornadoApi api = new TornadoApi("test-key");
        TornadoAgent agent = new TornadoAgent(api, ChatModel.OpenAi.Gpt41.V41Mini, tools: [MyTool]);

        Assert.That(agent.ToolList.ContainsKey("ToolName"), Is.True);
    }

    [Description("Description")]
    [ToolName("ToolName")]
    private static string MyTool([Description("Description")] string param1, [Description("Description")] string param2)
    {
        return $"Result: {param1} - {param2}";
    }
}

[TestFixture]
public class AdvancedToolDefinitionTests
{
    [Test]
    [Category("Docs:2. Agents/2. Tornado-Agent/4. Tools/1. Function-Tools.md#Advanced Tool definitions")]
    public void AddsMultipleTools()
    {
        TornadoApi api = new TornadoApi("test-key");
        TornadoAgent agent = new TornadoAgent(api, ChatModel.OpenAi.Gpt41.V41Mini);

        Tool toolA = new Tool((string input) => input, "tool_a", "Tool A");
        Tool toolB = new Tool((string input) => input, "tool_b", "Tool B");

        agent.AddTool(toolA);
        agent.AddTool(toolB);

        Assert.That(agent.ToolList.Count, Is.EqualTo(2));
    }
}

[TestFixture]
public class ToolPermissionTests
{
    [Test]
    [Category("Docs:2. Agents/2. Tornado-Agent/4. Tools/1. Function-Tools.md#Tool Permissions")]
    public void StoresPermissionFlags()
    {
        Dictionary<string, bool> permissions = [];
        permissions["dangerous_operation"] = true;
        permissions["safe_operation"] = false;

        Assert.That(permissions["dangerous_operation"], Is.True);
        Assert.That(permissions["safe_operation"], Is.False);
    }
}

[TestFixture]
public class ToolErrorHandlingTests
{
    [Test]
    [Category("Docs:2. Agents/2. Tornado-Agent/4. Tools/1. Function-Tools.md#Error Handling")]
    public void ReturnsErrorMessageOnFailure()
    {
        string result = SafeTool(null!);

        Assert.That(result.StartsWith("Error:"), Is.True);
    }

    private static string SafeTool(string input)
    {
        try
        {
            if (input is null)
            {
                throw new ArgumentNullException(nameof(input));
            }
            return "Success";
        }
        catch (Exception ex)
        {
            return $"Error: {ex.Message}";
        }
    }
}

[TestFixture]
public class ToolDocumentationTests
{
    [Test]
    [Category("Docs:2. Agents/2. Tornado-Agent/4. Tools/1. Function-Tools.md#Documentation")]
    public void UsesDefaultUnits()
    {
        string result = GetWeather("Prague");

        Assert.That(result.Contains("celsius"), Is.True);
    }

    private static string GetWeather(string city, string units = "celsius")
    {
        return $"Weather for {city} in {units}";
    }
}

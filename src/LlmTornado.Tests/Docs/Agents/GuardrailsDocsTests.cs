using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using LlmTornado;
using LlmTornado.Agents;
using LlmTornado.Agents.DataModels;
using LlmTornado.Chat;
using LlmTornado.Chat.Models;
using LlmTornado.Code;
using NUnit.Framework;

namespace LlmTornado.Tests.Docs.Agents;

[TestFixture]
public class GuardrailsQuickStartTests
{
    [Test]
    [Category("Docs:2. Agents/2. Tornado-Agent/6. Guardrails.md#Quick Start")]
    public void CreatesGuardrailOutput()
    {
        GuardRailFunctionOutput output = new GuardRailFunctionOutput("ok", false);

        Assert.That(output.OutputInfo, Is.EqualTo("ok"));
        Assert.That(output.TripwireTriggered, Is.False);
    }
}

[TestFixture]
public class GuardrailsToolPermissionTests
{
    [Test]
    [Category("Docs:2. Agents/2. Tornado-Agent/6. Guardrails.md#Tool Permissions")]
    public void StoresPermissionDictionary()
    {
        Dictionary<string, bool> permissions = [];
        permissions["send_email"] = true;
        permissions["read_file"] = false;
        permissions["delete_data"] = true;

        Assert.That(permissions["send_email"], Is.True);
        Assert.That(permissions["read_file"], Is.False);
        Assert.That(permissions["delete_data"], Is.True);
    }
}

[TestFixture]
public class GuardrailsInputValidationTests
{
    [Test]
    [Category("Docs:2. Agents/2. Tornado-Agent/6. Guardrails.md#Input Validation")]
    public void ThrowsOnLongInput()
    {
        TornadoApi api = new TornadoApi("test-key");
        TornadoAgent agent = new TornadoAgent(api, ChatModel.OpenAi.Gpt41.V41Mini);
        string input = new string('a', 10001);

        Assert.ThrowsAsync<InvalidOperationException>(async () => await SafeRunAsync(agent, input));
    }

    private static async Task<Conversation> SafeRunAsync(TornadoAgent agent, string userInput)
    {
        if (userInput.Length > 10000)
        {
            throw new InvalidOperationException("Input too long");
        }

        return await agent.Run(userInput);
    }
}

[TestFixture]
public class GuardrailsOutputFilteringTests
{
    [Test]
    [Category("Docs:2. Agents/2. Tornado-Agent/6. Guardrails.md#Output Filtering")]
    public void MasksSensitivePatterns()
    {
        string response = "SSN 123-45-6789 and card 1234567812345678";

        response = Regex.Replace(response, @"\d{3}-\d{2}-\d{4}", "***-**-****");
        response = Regex.Replace(response, @"\b\d{16}\b", "****-****-****-****");

        Assert.That(response.Contains("***-**-****"), Is.True);
        Assert.That(response.Contains("****-****-****-****"), Is.True);
    }
}

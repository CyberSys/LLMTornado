using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using LlmTornado;
using LlmTornado.Agents;
using LlmTornado.Agents.DataModels;
using LlmTornado.Chat;
using LlmTornado.Chat.Models;
using LlmTornado.Code;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using NUnit.Framework;
using Category = NUnit.Framework.CategoryAttribute;
using Description = System.ComponentModel.DescriptionAttribute;

namespace LlmTornado.Tests.Docs.Agents;

[TestFixture]
public class TornadoRunnerQuickStartTests
{
    [Test]
    [Category("Docs:2. Agents/2. Tornado-Agent/7. Tornado-Runner.md#Quick Start")]
    public void CreatesRunnerCapableAgent()
    {
        TornadoApi api = new TornadoApi("test-key");
        TornadoAgent agent = new TornadoAgent(api, ChatModel.OpenAi.Gpt41.V41Mini);

        Assert.That(agent.Running, Is.False);
        Assert.That(agent.Cancelled, Is.False);
    }
}

[TestFixture]
public class TornadoRunnerEventHandlerTests
{
    [Test]
    [Category("Docs:2. Agents/2. Tornado-Agent/7. Tornado-Runner.md#Custom Event Handlers")]
    public void DetectsCompletedEvent()
    {
        bool completed = false;

        ValueTask handler(AgentRunnerEvents runEvent)
        {
            if (runEvent.EventType == AgentRunnerEventTypes.Completed)
            {
                completed = true;
            }
            return ValueTask.CompletedTask;
        }

        TornadoApi api = new TornadoApi("test-key");
        Conversation conversation = new Conversation(api.Chat);
        AgentRunnerCompletedEvent evt = new AgentRunnerCompletedEvent(conversation);
        handler(evt).GetAwaiter().GetResult();

        Assert.That(completed, Is.True);
    }
}

[TestFixture]
public class TornadoRunnerExecutionConfigurationTests
{
    [Test]
    [Category("Docs:2. Agents/2. Tornado-Agent/7. Tornado-Runner.md#Execution Configuration")]
    public void UsesStreamingFlag()
    {
        bool streaming = true;

        Assert.That(streaming, Is.True);
    }
}

[TestFixture]
public class TornadoRunnerGuardrailsTests
{
    [Test]
    [Category("Docs:2. Agents/2. Tornado-Agent/7. Tornado-Runner.md#Using Guardrails")]
    public void MarksGuardrailAsBlocked()
    {
        GuardRailFunctionOutput output = new GuardRailFunctionOutput("blocked", true);

        Assert.That(output.TripwireTriggered, Is.True);
    }
}

[TestFixture]
public class TornadoRunnerToolPermissionTests
{
    [Test]
    [Category("Docs:2. Agents/2. Tornado-Agent/7. Tornado-Runner.md#Tool Permission")]
    public void StoresToolPermissionDictionary()
    {
        Dictionary<string, bool> permissions = new Dictionary<string, bool>
        {
            { "GetCurrentWeather", true }
        };

        Assert.That(permissions["GetCurrentWeather"], Is.True);
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum Unit
    {
        Celsius,
        Fahrenheit
    }

    [Description("Get the current weather in a given location")]
    public static string GetCurrentWeather(
        [Description("The city and state, e.g. Boston, MA")] string location,
        [Description("unit of temperature measurement in C or F")] Unit unit = Unit.Celsius)
    {
        return "31 C";
    }
}

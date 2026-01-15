using System.Collections.Generic;
using LlmTornado;
using LlmTornado.Agents;
using LlmTornado.Chat.Models;
using NUnit.Framework;

namespace LlmTornado.Tests.Docs.Agents;

[TestFixture]
public class StructuredOutputQuickStartTests
{
    [Test]
    [Category("Docs:2. Agents/2. Tornado-Agent/3. structured-output.md#Quick Start")]
    public void ConfiguresOutputSchema()
    {
        TornadoApi api = new TornadoApi("test-key");

        TornadoAgent agent = new TornadoAgent(
            client: api,
            model: ChatModel.OpenAi.Gpt41.V41Mini,
            instructions: "Extract contact information from text.",
            outputSchema: typeof(ContactInfo)
        );

        Assert.That(agent.OutputSchema, Is.EqualTo(typeof(ContactInfo)));
    }

    private class ContactInfo
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
    }
}

[TestFixture]
public class SchemaDefinitionTests
{
    [Test]
    [Category("Docs:2. Agents/2. Tornado-Agent/3. structured-output.md#Defining Schemas")]
    public void InitializesStepsCollection()
    {
        TaskBreakdown breakdown = new TaskBreakdown();

        Assert.That(breakdown.Steps, Is.Not.Null);
        Assert.That(breakdown.Steps.Count, Is.EqualTo(0));
    }

    private class TaskBreakdown
    {
        public List<string> Steps { get; set; } = [];
    }
}

[TestFixture]
public class DynamicSchemaTests
{
    [Test]
    [Category("Docs:2. Agents/2. Tornado-Agent/3. structured-output.md#Dynamic Schema Updates")]
    public void ClearsOutputSchema()
    {
        TornadoApi api = new TornadoApi("test-key");
        TornadoAgent agent = new TornadoAgent(api, ChatModel.OpenAi.Gpt41.V41Mini, outputSchema: typeof(TypeA));

        agent.UpdateOutputSchema(null);

        Assert.That(agent.OutputSchema, Is.Null);
    }

    private class TypeA
    {
        public string Name { get; set; } = string.Empty;
    }
}

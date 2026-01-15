using System.Collections.Generic;
using LlmTornado;
using LlmTornado.Agents;
using LlmTornado.Chat.Models;
using LlmTornado.Common;
using NUnit.Framework;

namespace LlmTornado.Tests.Docs.Agents;

[TestFixture]
public class AgentAsToolQuickStartTests
{
    [Test]
    [Category("Docs:2. Agents/2. Tornado-Agent/4. Tools/4. Agent-As-Tool.md#Quick Start")]
    public void AddsAgentToolToCoordinator()
    {
        TornadoApi api = new TornadoApi("test-key");
        TornadoAgent weatherAgent = new TornadoAgent(api, ChatModel.OpenAi.Gpt41.V41Mini, name: "WeatherAgent");
        TornadoAgent coordinator = new TornadoAgent(api, ChatModel.OpenAi.Gpt41.V41, name: "Coordinator");

        Tool weatherTool = weatherAgent.AsTool();
        coordinator.AddTool(weatherTool);

        Assert.That(coordinator.ToolList.Count, Is.EqualTo(1));
    }
}

[TestFixture]
public class AgentAsToolBasicTests
{
    [Test]
    [Category("Docs:2. Agents/2. Tornado-Agent/4. Tools/4. Agent-As-Tool.md#Basic Agent Tool")]
    public void RegistersMathAgentTool()
    {
        TornadoApi api = new TornadoApi("test-key");
        TornadoAgent mathAgent = new TornadoAgent(api, ChatModel.OpenAi.Gpt41.V41Mini, name: "MathExpert");
        TornadoAgent mainAgent = new TornadoAgent(api, ChatModel.OpenAi.Gpt41.V41);

        mainAgent.AddTool(mathAgent.AsTool());

        Assert.That(mainAgent.ToolList.Count, Is.EqualTo(1));
    }
}

[TestFixture]
public class AgentAsToolMultipleTests
{
    [Test]
    [Category("Docs:2. Agents/2. Tornado-Agent/4. Tools/4. Agent-As-Tool.md#Multiple Agent Tools")]
    public void RegistersMultipleAgentTools()
    {
        TornadoApi api = new TornadoApi("test-key");
        ChatModel model = ChatModel.OpenAi.Gpt41.V41;

        TornadoAgent codeAgent = new TornadoAgent(api, model, name: "CodeExpert");
        TornadoAgent securityAgent = new TornadoAgent(api, model, name: "SecurityExpert");
        TornadoAgent architectAgent = new TornadoAgent(api, model, name: "ArchitectExpert");

        TornadoAgent coordinator = new TornadoAgent(api, model, name: "TechLeadCoordinator");
        List<Tool> tools = [codeAgent.AsTool(), securityAgent.AsTool(), architectAgent.AsTool()];

        coordinator.AddTool(tools);

        Assert.That(coordinator.ToolList.Count, Is.EqualTo(3));
    }
}

[TestFixture]
public class AgentAsToolTaskDecompositionTests
{
    [Test]
    [Category("Docs:2. Agents/2. Tornado-Agent/4. Tools/4. Agent-As-Tool.md#Task Decomposition")]
    public void AddsWorkflowTools()
    {
        TornadoApi api = new TornadoApi("test-key");
        ChatModel model = ChatModel.OpenAi.Gpt41.V41;

        TornadoAgent researcher = new TornadoAgent(api, model, name: "Researcher");
        TornadoAgent analyzer = new TornadoAgent(api, model, name: "Analyzer");
        TornadoAgent writer = new TornadoAgent(api, model, name: "Writer");

        TornadoAgent projectManager = new TornadoAgent(api, model, name: "ProjectManager");
        projectManager.AddTool([
            researcher.AsTool(),
            analyzer.AsTool(),
            writer.AsTool()
        ]);

        Assert.That(projectManager.ToolList.Count, Is.EqualTo(3));
    }
}

[TestFixture]
public class AgentAsToolDomainSpecializationTests
{
    [Test]
    [Category("Docs:2. Agents/2. Tornado-Agent/4. Tools/4. Agent-As-Tool.md#Domain Specialization")]
    public void RegistersSpecialistAgents()
    {
        TornadoApi api = new TornadoApi("test-key");
        ChatModel model = ChatModel.OpenAi.Gpt41.V41;

        TornadoAgent legalAgent = new TornadoAgent(api, model, name: "LegalAdvisor", outputSchema: typeof(LegalReview));
        TornadoAgent financeAgent = new TornadoAgent(api, model, name: "FinancialAdvisor", outputSchema: typeof(FinancialAnalysis));
        TornadoAgent businessAgent = new TornadoAgent(api, model, name: "BusinessAdvisor");

        businessAgent.AddTool([legalAgent.AsTool(), financeAgent.AsTool()]);

        Assert.That(businessAgent.ToolList.Count, Is.EqualTo(2));
    }

    private class LegalReview
    {
        public string Summary { get; set; } = string.Empty;
    }

    private class FinancialAnalysis
    {
        public string Summary { get; set; } = string.Empty;
    }
}

[TestFixture]
public class AgentAsToolDynamicRegistrationTests
{
    [Test]
    [Category("Docs:2. Agents/2. Tornado-Agent/4. Tools/4. Agent-As-Tool.md#Dynamic Agent Tool Registration")]
    public void AddsSpecialistTool()
    {
        TornadoApi api = new TornadoApi("test-key");
        TornadoAgent specialistAgent = new TornadoAgent(api, ChatModel.OpenAi.Gpt41.V41Mini);
        TornadoAgent coordinator = new TornadoAgent(api, ChatModel.OpenAi.Gpt41.V41);

        coordinator.AddTool(specialistAgent.AsTool());

        Assert.That(coordinator.ToolList.Count, Is.EqualTo(1));
    }
}

[TestFixture]
public class AgentAsToolConditionalUseTests
{
    [Test]
    [Category("Docs:2. Agents/2. Tornado-Agent/4. Tools/4. Agent-As-Tool.md#Conditional Agent Use")]
    public void AddsSpecialistToolList()
    {
        TornadoApi api = new TornadoApi("test-key");
        ChatModel model = ChatModel.OpenAi.Gpt41.V41;

        TornadoAgent codeAgent = new TornadoAgent(api, model, name: "CodeExpert");
        TornadoAgent securityAgent = new TornadoAgent(api, model, name: "SecurityExpert");
        TornadoAgent architectAgent = new TornadoAgent(api, model, name: "ArchitectExpert");

        List<Tool> specialistTools = [
            codeAgent.AsTool(),
            securityAgent.AsTool(),
            architectAgent.AsTool()
        ];

        TornadoAgent coordinator = new TornadoAgent(api, model, instructions: "Route tasks");
        coordinator.AddTool(specialistTools);

        Assert.That(coordinator.ToolList.Count, Is.EqualTo(3));
    }
}

using System;
using LlmTornado.ChatFunctions;
using LlmTornado.Code;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LlmTornado.Chat.Vendors.MoonshotAi;

/// <summary>
/// https://platform.moonshot.ai/docs/api/chat
/// </summary>
internal class VendorMoonshotAiChatRequest
{
    public VendorMoonshotAiChatRequestData? ExtendedRequest { get; set; }
    public ChatRequest? NativeRequest { get; set; }
    
    [JsonIgnore]
    public ChatRequest SourceRequest { get; set; }
    
    public JObject Serialize(JsonSerializerSettings settings)
    {
        JsonSerializer serializer = JsonSerializer.CreateDefault(settings);
        return JObject.FromObject(ExtendedRequest ?? NativeRequest, serializer);
    }
    
    public VendorMoonshotAiChatRequest(ChatRequest request, IEndpointProvider provider)
    {
        SourceRequest = request;
        
        // Kimi API does not support tool_choice=required
        if (request.ToolChoice == OutboundToolChoice.Required)
        {
            request.ToolChoice = OutboundToolChoice.Auto;
        }
        
        bool isK25Model = request.Model?.Name?.Contains("k2.5", StringComparison.OrdinalIgnoreCase) == true;
        
        if (isK25Model)
        {
            // K2.5 has fixed parameters - clear them to avoid API errors
            request.Temperature = null;
            request.TopP = null;
            request.NumChoicesPerMessage = null;
            request.PresencePenalty = null;
            request.FrequencyPenalty = null;
            
            // Use extended request to add thinking parameter
            ExtendedRequest = new VendorMoonshotAiChatRequestData(request)
            {
                Thinking = new VendorMoonshotAiThinking
                {
                    Type = request.ReasoningBudget == 0 ? "disabled" : "enabled"
                }
            };
        }
        else
        {
            // Temperature clamping [0, 1] for non-K2.5 models
            if (request.Temperature is not null)
            {
                request.Temperature = Math.Clamp(request.Temperature.Value, 0, 1);
            }
            
            NativeRequest = request;
        }
    }
}

/// <summary>
/// Extended chat request data for MoonshotAi with vendor-specific fields.
/// </summary>
internal class VendorMoonshotAiChatRequestData : ChatRequest
{
    /// <summary>
    /// Controls thinking mode for kimi-k2.5 model.
    /// </summary>
    [JsonProperty("thinking")]
    public VendorMoonshotAiThinking? Thinking { get; set; }
    
    public VendorMoonshotAiChatRequestData(ChatRequest request) : base(request)
    {
    }
}

/// <summary>
/// Thinking parameter for Kimi K2.5 model.
/// </summary>
internal class VendorMoonshotAiThinking
{
    /// <summary>
    /// "enabled" or "disabled"
    /// </summary>
    [JsonProperty("type")]
    public string Type { get; set; } = "enabled";
}

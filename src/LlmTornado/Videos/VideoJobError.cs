using Newtonsoft.Json;

namespace LlmTornado.Videos;

/// <summary>
/// Error information for a failed video job.
/// </summary>
public class VideoJobError
{
    /// <summary>
    /// Error code.
    /// </summary>
    [JsonProperty("code")]
    public string? Code { get; set; }
    
    /// <summary>
    /// Human-readable error message.
    /// </summary>
    [JsonProperty("message")]
    public string? Message { get; set; }
}

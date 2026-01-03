using System;
using LlmTornado.Code;
using Newtonsoft.Json;

namespace LlmTornado.Videos;

/// <summary>
/// Represents a video generation job. Harmonized across providers.
/// </summary>
public class VideoJob : ApiResultBase
{
    /// <summary>
    /// Unique identifier for the video job.
    /// </summary>
    [JsonProperty("id")]
    public string Id { get; set; } = string.Empty;
    
    /// <summary>
    /// The object type. Usually "video".
    /// </summary>
    [JsonProperty("object")]
    public string? Object { get; set; }
    
    /// <summary>
    /// The video generation model used.
    /// </summary>
    [JsonProperty("model")]
    public string? Model { get; set; }
    
    /// <summary>
    /// Current lifecycle status of the video job.
    /// </summary>
    [JsonProperty("status")]
    public VideoJobStatus Status { get; set; }
    
    /// <summary>
    /// Approximate completion percentage (0-100).
    /// </summary>
    [JsonProperty("progress")]
    public int? Progress { get; set; }
    
    /// <summary>
    /// The prompt used to generate the video.
    /// </summary>
    [JsonProperty("prompt")]
    public string? Prompt { get; set; }
    
    /// <summary>
    /// Duration of the generated clip in seconds.
    /// </summary>
    [JsonProperty("seconds")]
    public string? Seconds { get; set; }
    
    /// <summary>
    /// Resolution of the generated video.
    /// </summary>
    [JsonProperty("size")]
    public string? Size { get; set; }
    
    /// <summary>
    /// Quality level of the video.
    /// </summary>
    [JsonProperty("quality")]
    public string? Quality { get; set; }
    
    /// <summary>
    /// Identifier of the source video if this is a remix.
    /// </summary>
    [JsonProperty("remixed_from_video_id")]
    public string? RemixedFromVideoId { get; set; }
    
    /// <summary>
    /// Error information if the job failed.
    /// </summary>
    [JsonProperty("error")]
    public VideoJobError? Error { get; set; }
    
    // Raw timestamp fields for deserialization
    [JsonProperty("created_at")]
    internal object? CreatedAtRaw { get; set; }
    
    [JsonProperty("completed_at")]
    internal object? CompletedAtRaw { get; set; }
    
    [JsonProperty("expires_at")]
    internal object? ExpiresAtRaw { get; set; }
    
    /// <summary>
    /// When the job was created.
    /// </summary>
    [JsonIgnore]
    public DateTime? CreatedAt => ParseDateTime(CreatedAtRaw);
    
    /// <summary>
    /// When the job completed.
    /// </summary>
    [JsonIgnore]
    public DateTime? CompletedAt => ParseDateTime(CompletedAtRaw);
    
    /// <summary>
    /// When the downloadable assets expire.
    /// </summary>
    [JsonIgnore]
    public DateTime? ExpiresAt => ParseDateTime(ExpiresAtRaw);
    
    /// <summary>
    /// Provider that generated this job.
    /// </summary>
    [JsonIgnore]
    public LLmProviders SourceProvider { get; set; }
    
    /// <summary>
    /// Whether the video generation is complete (either succeeded or failed).
    /// Harmonized across providers.
    /// </summary>
    [JsonIgnore]
    public bool Done => Status is VideoJobStatus.Completed or VideoJobStatus.Failed;
    
    /// <summary>
    /// Direct URI to download the video content. Only populated for providers that return
    /// a download URL (e.g., Google). For other providers like OpenAI, use the
    /// <c>DownloadContent</c> method on the endpoint instead.
    /// </summary>
    [JsonIgnore]
    public string? VideoUri { get; set; }
    
    // --- Internal provider-specific fields ---
    
    /// <summary>
    /// Operation name for Google/Gemini. Used internally for polling status.
    /// </summary>
    [JsonIgnore]
    internal string? OperationName { get; set; }
    
    private static DateTime? ParseDateTime(object? value)
    {
        return value switch
        {
            null => null,
            long unixTimestamp => DateTimeOffset.FromUnixTimeSeconds(unixTimestamp).UtcDateTime,
            int unixTimestamp => DateTimeOffset.FromUnixTimeSeconds(unixTimestamp).UtcDateTime,
            double unixTimestamp => DateTimeOffset.FromUnixTimeSeconds((long)unixTimestamp).UtcDateTime,
            string dateString when DateTime.TryParse(dateString, out DateTime parsed) => parsed,
            _ => null
        };
    }
}

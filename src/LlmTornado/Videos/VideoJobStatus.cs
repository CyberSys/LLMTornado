using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace LlmTornado.Videos;

/// <summary>
/// Status of a video generation job.
/// </summary>
[JsonConverter(typeof(StringEnumConverter))]
public enum VideoJobStatus
{
    /// <summary>
    /// Unknown status.
    /// </summary>
    [EnumMember(Value = "unknown")]
    Unknown,
    
    /// <summary>
    /// Job is queued and waiting to be processed.
    /// </summary>
    [EnumMember(Value = "queued")]
    Queued,
    
    /// <summary>
    /// Job is currently being processed.
    /// </summary>
    [EnumMember(Value = "in_progress")]
    InProgress,
    
    /// <summary>
    /// Job has completed successfully.
    /// </summary>
    [EnumMember(Value = "completed")]
    Completed,
    
    /// <summary>
    /// Job has failed.
    /// </summary>
    [EnumMember(Value = "failed")]
    Failed
}

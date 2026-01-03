using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace LlmTornado.Videos;

/// <summary>
/// Type of content to download from a completed video job.
/// </summary>
[JsonConverter(typeof(StringEnumConverter))]
public enum VideoContentVariant
{
    /// <summary>
    /// The MP4 video file. Default.
    /// </summary>
    [EnumMember(Value = "video")]
    Video,
    
    /// <summary>
    /// A thumbnail image (WebP format for OpenAI).
    /// </summary>
    [EnumMember(Value = "thumbnail")]
    Thumbnail,
    
    /// <summary>
    /// A spritesheet image (JPG format for OpenAI).
    /// </summary>
    [EnumMember(Value = "spritesheet")]
    Spritesheet
}

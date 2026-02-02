using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace LlmTornado.Videos.Vendors.Zai;

/// <summary>
/// Z.AI-specific extensions for video generation requests.
/// </summary>
public class VideoZaiExtensions
{
    /// <summary>
    /// Output mode for CogVideoX. Defaults to <see cref="VideoZaiQuality.Speed"/>.
    /// </summary>
    public VideoZaiQuality? Quality { get; set; }
    
    /// <summary>
    /// Video frame rate (FPS) for CogVideoX. Valid values are 30 or 60. Defaults to 30.
    /// </summary>
    public int? Fps { get; set; }
    
    /// <summary>
    /// Whether to generate AI sound effects/background music. Defaults to false.
    /// Supported by CogVideoX and Vidu image/start-end/reference models.
    /// </summary>
    public bool? WithAudio { get; set; }
    
    /// <summary>
    /// Style for Vidu text-to-video generation. Defaults to <see cref="VideoZaiStyle.General"/>.
    /// Only applicable to viduq1-text model.
    /// </summary>
    public VideoZaiStyle? Style { get; set; }
    
    /// <summary>
    /// Motion amplitude for Vidu models. Controls how much movement appears in the video.
    /// Defaults to <see cref="VideoZaiMovementAmplitude.Auto"/>.
    /// </summary>
    public VideoZaiMovementAmplitude? MovementAmplitude { get; set; }
    
    /// <summary>
    /// Unique request ID for tracking. If not provided, the platform will generate one.
    /// </summary>
    public string? RequestId { get; set; }
    
    /// <summary>
    /// Unique ID of the end-user for abuse monitoring. Length: 6-128 characters.
    /// </summary>
    public string? UserId { get; set; }
}

/// <summary>
/// Quality/speed mode for CogVideoX video generation.
/// </summary>
[JsonConverter(typeof(StringEnumConverter))]
public enum VideoZaiQuality
{
    /// <summary>
    /// Prioritizes speed - faster generation time, relatively lower quality.
    /// </summary>
    [EnumMember(Value = "speed")]
    Speed,
    
    /// <summary>
    /// Prioritizes quality - higher generation quality, slower generation time.
    /// </summary>
    [EnumMember(Value = "quality")]
    Quality
}

/// <summary>
/// Style options for Vidu text-to-video generation.
/// </summary>
[JsonConverter(typeof(StringEnumConverter))]
public enum VideoZaiStyle
{
    /// <summary>
    /// General style - can be controlled using prompts to define the style.
    /// </summary>
    [EnumMember(Value = "general")]
    General,
    
    /// <summary>
    /// Anime style - optimized for anime-specific visuals.
    /// </summary>
    [EnumMember(Value = "anime")]
    Anime
}

/// <summary>
/// Motion amplitude options for Vidu video generation.
/// </summary>
[JsonConverter(typeof(StringEnumConverter))]
public enum VideoZaiMovementAmplitude
{
    /// <summary>
    /// Automatic motion amplitude based on the content.
    /// </summary>
    [EnumMember(Value = "auto")]
    Auto,
    
    /// <summary>
    /// Small/subtle motion.
    /// </summary>
    [EnumMember(Value = "small")]
    Small,
    
    /// <summary>
    /// Medium motion.
    /// </summary>
    [EnumMember(Value = "medium")]
    Medium,
    
    /// <summary>
    /// Large/dramatic motion.
    /// </summary>
    [EnumMember(Value = "large")]
    Large
}

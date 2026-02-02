using System.Collections.Generic;

namespace LlmTornado.Audio.Vendors.Zai;

/// <summary>
/// Z.AI-specific extensions for audio transcription requests.
/// </summary>
public class TranscriptionRequestZaiExtensions
{
    /// <summary>
    /// Hotword list to improve recognition accuracy for domain-specific vocabulary.
    /// Format example: ["person_name", "place_name"]. Recommended not to exceed 100 items.
    /// </summary>
    public List<string>? Hotwords { get; set; }
    
    /// <summary>
    /// Unique request ID for tracking. If not provided, the platform will generate one.
    /// Must be unique to distinguish each request.
    /// </summary>
    public string? RequestId { get; set; }
    
    /// <summary>
    /// Unique ID of the end-user for abuse monitoring and intervention.
    /// Length requirement: 6-128 characters.
    /// </summary>
    public string? UserId { get; set; }
    
    /// <summary>
    /// Base64 encoded audio file. Alternative to providing the file directly.
    /// If both file and file_base64 are provided, file takes precedence.
    /// </summary>
    public string? FileBase64 { get; set; }
}

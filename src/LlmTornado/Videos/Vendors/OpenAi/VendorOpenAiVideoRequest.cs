using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Runtime.Serialization;
using LlmTornado.Videos.Models;

namespace LlmTornado.Videos.Vendors.OpenAi;

/// <summary>
/// Handles serialization of video generation requests for OpenAI.
/// Maps harmonized VideoGenerationRequest to OpenAI's format.
/// </summary>
internal static class VendorOpenAiVideoRequest
{
    /// <summary>
    /// Serializes a harmonized video generation request to OpenAI's multipart/form-data content.
    /// </summary>
    public static MultipartFormDataContent Serialize(VideoGenerationRequest request)
    {
        MultipartFormDataContent content = new MultipartFormDataContent();
        
        // Required: prompt
        if (!string.IsNullOrEmpty(request.Prompt))
        {
            content.Add(new StringContent(request.Prompt), "prompt");
        }
        
        // Model - use OpenAI default if not set
        string modelName = request.Model?.Name ?? VideoModel.OpenAi.Sora.Sora2.Name;
        content.Add(new StringContent(modelName), "model");
        
        // Map harmonized Duration to OpenAI's seconds
        string? seconds = MapDurationToSeconds(request.Duration, request.DurationSeconds);
        if (seconds is not null)
        {
            content.Add(new StringContent(seconds), "seconds");
        }
        
        // Map harmonized AspectRatio + Resolution to OpenAI's size
        string? size = MapToSize(request.AspectRatio, request.Resolution);
        if (size is not null)
        {
            content.Add(new StringContent(size), "size");
        }
        
        // Map harmonized Image to OpenAI's input_reference
        if (request.Image is not null)
        {
            byte[]? imageBytes = GetImageBytes(request.Image);
            if (imageBytes is not null)
            {
                ByteArrayContent imageContent = new ByteArrayContent(imageBytes);
                string mimeType = request.Image.MimeType ?? "image/jpeg";
                imageContent.Headers.ContentType = new MediaTypeHeaderValue(mimeType);
                content.Add(imageContent, "input_reference", "input_reference.jpg");
            }
        }
        
        // OpenAI-specific extensions override harmonized values
        if (request.OpenAiExtensions is not null)
        {
            VideoOpenAiExtensions ext = request.OpenAiExtensions;
        }
        
        return content;
    }
    
    /// <summary>
    /// Maps harmonized VideoDuration to OpenAI seconds string.
    /// </summary>
    private static string? MapDurationToSeconds(VideoDuration? duration, int? customSeconds)
    {
        if (customSeconds.HasValue)
        {
            // OpenAI only supports 4, 8, 12 - pick closest
            return customSeconds.Value switch
            {
                <= 4 => "4",
                <= 8 => "8",
                _ => "12"
            };
        }
        
        if (duration is null)
        {
            return null;
        }
        
        return duration.Value switch
        {
            VideoDuration.Seconds4 => "4",
            VideoDuration.Seconds5 => "4",
            VideoDuration.Seconds6 => "8",
            VideoDuration.Seconds8 => "8",
            VideoDuration.Seconds12 => "12",
            _ => null
        };
    }
    
    /// <summary>
    /// Maps harmonized AspectRatio + Resolution to OpenAI size string.
    /// </summary>
    private static string? MapToSize(VideoAspectRatio? aspectRatio, VideoResolution? resolution)
    {
        if (aspectRatio is null)
        {
            return null;
        }
        
        // OpenAI sizes: 720x1280, 1280x720, 1024x1792, 1792x1024
        // Map based on aspect ratio (resolution is less important for OpenAI)
        return aspectRatio.Value switch
        {
            VideoAspectRatio.Portrait => resolution == VideoResolution.FullHD ? "1024x1792" : "720x1280",
            VideoAspectRatio.Widescreen => resolution == VideoResolution.FullHD ? "1792x1024" : "1280x720",
            _ => null
        };
    }
    
    /// <summary>
    /// Gets image bytes from VideoImage (handles base64).
    /// </summary>
    private static byte[]? GetImageBytes(VideoImage image)
    {
        if (string.IsNullOrEmpty(image.Url))
        {
            return null;
        }
        
        // If it looks like base64, decode it
        try
        {
            // Check if it's a data URL
            if (image.Url.StartsWith("data:"))
            {
                int commaIndex = image.Url.IndexOf(',');
                if (commaIndex > 0)
                {
                    return Convert.FromBase64String(image.Url.Substring(commaIndex + 1));
                }
            }
            
            // Try to decode as raw base64
            return Convert.FromBase64String(image.Url);
        }
        catch
        {
            // Not base64, might be a URL - OpenAI requires file upload, not URL
            return null;
        }
    }
    
    /// <summary>
    /// Gets the EnumMember value for an enum.
    /// </summary>
    private static string GetEnumValue<T>(T value) where T : Enum
    {
        FieldInfo? field = typeof(T).GetField(value.ToString());
        EnumMemberAttribute? attribute = field?.GetCustomAttribute<EnumMemberAttribute>(false);
        return attribute?.Value ?? value.ToString();
    }
}

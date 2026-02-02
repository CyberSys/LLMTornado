using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using LlmTornado.Audio.Models.Zai;
using LlmTornado.Code;
using Newtonsoft.Json;

namespace LlmTornado.Audio.Vendors.Zai;

/// <summary>
/// Handles audio transcription operations for Z.AI.
/// </summary>
internal static class VendorZaiAudioHandler
{
    private static readonly JsonSerializerSettings SerializerSettings = new JsonSerializerSettings
    {
        NullValueHandling = NullValueHandling.Ignore
    };
    
    /// <summary>
    /// Creates a multipart form request for Z.AI audio transcription.
    /// </summary>
    public static MultipartFormDataContent SerializeRequest(TranscriptionRequest request)
    {
        MultipartFormDataContent content = new MultipartFormDataContent();
        
        // Model
        content.Add(new StringContent(request.Model?.Name ?? AudioModelZaiAsr.ModelGlmAsr2512.Name), "model");
        
        // Stream
        if (request.Stream ?? false)
        {
            content.Add(new StringContent("true"), "stream");
        }
        
        // Prompt
        if (!string.IsNullOrWhiteSpace(request.Prompt))
        {
            content.Add(new StringContent(request.Prompt), "prompt");
        }
        
        // File (binary)
        if (request.File?.Data is not null)
        {
            MemoryStream ms = new MemoryStream(request.File.Data);
            StreamContent sc = new StreamContent(ms);
            sc.Headers.ContentLength = request.File.Data.Length;
            sc.Headers.ContentType = new MediaTypeHeaderValue(request.File.GetContentType);
            content.Add(sc, "file", GetFileName(request.File.ContentType));
        }
        else if (request.File?.File is not null)
        {
            StreamContent sc = new StreamContent(request.File.File);
            sc.Headers.ContentLength = request.File.File.Length;
            sc.Headers.ContentType = new MediaTypeHeaderValue(request.File.GetContentType);
            content.Add(sc, "file", GetFileName(request.File.ContentType));
        }
        
        // Z.AI-specific extensions
        if (request.ZaiExtensions is not null)
        {
            TranscriptionRequestZaiExtensions ext = request.ZaiExtensions;
            
            // Hotwords - array of strings
            if (ext.Hotwords is { Count: > 0 })
            {
                string hotwordsJson = JsonConvert.SerializeObject(ext.Hotwords, SerializerSettings);
                content.Add(new StringContent(hotwordsJson), "hotwords");
            }
            
            // Request ID
            if (!string.IsNullOrEmpty(ext.RequestId))
            {
                content.Add(new StringContent(ext.RequestId), "request_id");
            }
            
            // User ID
            if (!string.IsNullOrEmpty(ext.UserId))
            {
                content.Add(new StringContent(ext.UserId), "user_id");
            }
            
            // File Base64 (alternative to binary file)
            if (!string.IsNullOrEmpty(ext.FileBase64))
            {
                content.Add(new StringContent(ext.FileBase64), "file_base64");
            }
        }
        
        return content;
    }
    
    /// <summary>
    /// Creates a transcription using Z.AI's API.
    /// </summary>
    public static async Task<TranscriptionResult?> CreateTranscription(
        TranscriptionRequest request,
        IEndpointProvider provider,
        EndpointBase endpoint,
        CancellationToken cancellationToken)
    {
        string url = provider.ApiUrl(CapabilityEndpoints.Audio, "/transcriptions");
        
        using MultipartFormDataContent content = SerializeRequest(request);
        
        TranscriptionResult? result = await endpoint.HttpPost1<TranscriptionResult>(
            provider,
            CapabilityEndpoints.Audio,
            url,
            content,
            request.Model,
            request,
            cancellationToken
        ).ConfigureAwait(false);
        
        return result;
    }
    
    /// <summary>
    /// Creates a streaming transcription request for Z.AI's API.
    /// Returns the serialized multipart content for streaming.
    /// </summary>
    public static MultipartFormDataContent CreateStreamingRequest(TranscriptionRequest request)
    {
        request.Stream = true;
        return SerializeRequest(request);
    }
    
    /// <summary>
    /// Maps Z.AI transcription response to TranscriptionResult.
    /// </summary>
    public static TranscriptionResult MapResponse(VendorZaiAudioResponse response)
    {
        return new TranscriptionResult
        {
            Text = response.Text ?? string.Empty,
            Task = "transcribe"
        };
    }
    
    /// <summary>
    /// Maps Z.AI streaming event to TranscriptionResult.
    /// </summary>
    public static TranscriptionResult? MapStreamEvent(VendorZaiAudioStreamResponse streamEvent)
    {
        if (streamEvent.Type == "transcript.text.delta")
        {
            return new TranscriptionResult
            {
                Text = streamEvent.Delta ?? string.Empty,
                EventType = AudioStreamEventTypes.TranscriptDelta
            };
        }
        
        if (streamEvent.Type == "transcript.text.done")
        {
            return new TranscriptionResult
            {
                Text = streamEvent.Text ?? string.Empty,
                EventType = AudioStreamEventTypes.TranscriptDone
            };
        }
        
        return null;
    }
    
    private static string GetFileName(AudioFileTypes contentType)
    {
        return contentType switch
        {
            AudioFileTypes.Wav => "audio.wav",
            AudioFileTypes.Mp3 => "audio.mp3",
            AudioFileTypes.Flac => "audio.flac",
            AudioFileTypes.Mp4 => "audio.mp4",
            AudioFileTypes.Mpeg => "audio.mpeg",
            AudioFileTypes.Mpga => "audio.mpga",
            AudioFileTypes.M4a => "audio.m4a",
            AudioFileTypes.Ogg => "audio.ogg",
            AudioFileTypes.Webm => "audio.webm",
            _ => "audio.wav"
        };
    }
}

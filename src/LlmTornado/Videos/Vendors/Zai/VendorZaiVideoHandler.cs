using System.Threading;
using System.Threading.Tasks;
using LlmTornado.Code;
using LlmTornado.Common;
using Newtonsoft.Json;

namespace LlmTornado.Videos.Vendors.Zai;

/// <summary>
/// Handles all video operations for Z.AI.
/// </summary>
internal static class VendorZaiVideoHandler
{
    private static readonly JsonSerializerSettings SerializerSettings = new JsonSerializerSettings
    {
        NullValueHandling = NullValueHandling.Ignore
    };
    
    /// <summary>
    /// Creates a new video generation job.
    /// </summary>
    public static async Task<HttpCallResult<VideoJob>> Create(
        VideoGenerationRequest request, 
        IEndpointProvider provider, 
        EndpointBase endpoint, 
        CancellationToken cancellationToken)
    {
        VendorZaiVideoGenerationRequest zaiRequest = VendorZaiVideoGenerationRequest.FromRequest(request);
        string json = JsonConvert.SerializeObject(zaiRequest, SerializerSettings);
        
        // Z.AI uses /paas/v4/videos/generations endpoint
        string url = endpoint.GetUrl(provider, "/generations");
        
        HttpCallResult<VendorZaiVideoCreateResponse> result = await endpoint.HttpPost<VendorZaiVideoCreateResponse>(
            provider, 
            CapabilityEndpoints.Videos, 
            url,
            postData: json,
            ct: cancellationToken
        ).ConfigureAwait(false);
        
        if (!result.Ok || result.Data is null)
        {
            return new HttpCallResult<VideoJob>(result.Code, result.Response, null, false, result.Request)
            {
                Exception = result.Exception
            };
        }
        
        // Convert to VideoJob
        VideoJob job = new VideoJob
        {
            Id = result.Data.Id,
            Status = MapTaskStatus(result.Data.TaskStatus),
            Model = request.Model?.Name,
            Prompt = request.Prompt,
            SourceProvider = LLmProviders.Zai
        };
        
        return new HttpCallResult<VideoJob>(result.Code, result.Response, job, true, result.Request);
    }
    
    /// <summary>
    /// Retrieves the status of a video job.
    /// </summary>
    public static async Task<HttpCallResult<VideoJob>> Get(
        string taskId, 
        IEndpointProvider provider, 
        EndpointBase endpoint, 
        CancellationToken cancellationToken)
    {
        // Z.AI uses /paas/v4/async-result/{id} endpoint for status
        // Note: This is a different path from the videos endpoint
        string baseUrl = provider.ApiUrl(CapabilityEndpoints.Videos, null);
        // Replace /videos with /async-result
        string asyncResultUrl = baseUrl.Replace("/videos", "/async-result");
        string url = $"{asyncResultUrl}/{taskId}";
        
        HttpCallResult<VendorZaiVideoStatusResponse> result = await endpoint.HttpGet<VendorZaiVideoStatusResponse>(
            provider, 
            CapabilityEndpoints.None, // Use None since we're using a custom URL
            url,
            ct: cancellationToken
        ).ConfigureAwait(false);
        
        if (!result.Ok || result.Data is null)
        {
            return new HttpCallResult<VideoJob>(result.Code, result.Response, null, false, result.Request)
            {
                Exception = result.Exception
            };
        }
        
        // Convert to VideoJob
        VideoJob job = new VideoJob
        {
            Id = taskId,
            Model = result.Data.Model,
            SourceProvider = LLmProviders.Zai
        };
        
        // Map Z.AI status to harmonized VideoJobStatus
        job.Status = MapTaskStatus(result.Data.TaskStatus);
        
        // Get video details if available
        if (result.Data.VideoResult is { Count: > 0 })
        {
            VendorZaiVideoResultData video = result.Data.VideoResult[0];
            job.VideoUri = video.Url;
            job.CoverImageUri = video.CoverImageUrl;
        }
        
        return new HttpCallResult<VideoJob>(result.Code, result.Response, job, true, result.Request);
    }
    
    /// <summary>
    /// Downloads video content from the VideoUri.
    /// </summary>
    public static async Task<StreamResponse?> GetContent(
        string videoUri,
        IEndpointProvider provider, 
        EndpointBase endpoint, 
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(videoUri))
        {
            return null;
        }
        
        // For Z.AI, the video URL is returned directly - we need to fetch it
        return await endpoint.HttpGetStream(
            provider,
            CapabilityEndpoints.None,
            videoUri,
            ct: cancellationToken
        ).ConfigureAwait(false);
    }
    
    private static VideoJobStatus MapTaskStatus(string? status)
    {
        if (string.IsNullOrEmpty(status))
        {
            return VideoJobStatus.Unknown;
        }
        
        return status.ToUpperInvariant() switch
        {
            "PROCESSING" => VideoJobStatus.InProgress,
            "SUCCESS" => VideoJobStatus.Completed,
            "FAIL" => VideoJobStatus.Failed,
            _ => VideoJobStatus.Unknown
        };
    }
}

using System.Threading;
using System.Threading.Tasks;
using LlmTornado.Code;
using LlmTornado.Common;
using Newtonsoft.Json;

namespace LlmTornado.Videos.Vendors.XAi;

/// <summary>
/// Handles all video operations for xAI.
/// </summary>
internal static class VendorXAiVideoHandler
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
        VendorXAiVideoGenerationRequest xaiRequest = VendorXAiVideoGenerationRequest.FromRequest(request);
        string json = JsonConvert.SerializeObject(xaiRequest, SerializerSettings);
        
        // xAI uses /v1/videos/generations endpoint
        string url = endpoint.GetUrl(provider, "/generations");
        
        HttpCallResult<VendorXAiVideoCreateResponse> result = await endpoint.HttpPost<VendorXAiVideoCreateResponse>(
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
            Id = result.Data.RequestId,
            Status = VideoJobStatus.Queued, // xAI returns "pending" which maps to Queued
            Model = request.Model?.Name,
            Prompt = request.Prompt,
            SourceProvider = LLmProviders.XAi
        };
        
        return new HttpCallResult<VideoJob>(result.Code, result.Response, job, true, result.Request);
    }
    
    /// <summary>
    /// Creates a new video edit job.
    /// </summary>
    public static async Task<HttpCallResult<VideoJob>> Edit(
        string prompt,
        string videoUrl,
        VideoGenerationRequest request,
        IEndpointProvider provider, 
        EndpointBase endpoint, 
        CancellationToken cancellationToken)
    {
        VendorXAiVideoEditRequest xaiRequest = new VendorXAiVideoEditRequest
        {
            Prompt = prompt,
            Video = new VendorXAiVideoInputVideo { Url = videoUrl },
            Model = request.Model?.Name
        };
        
        if (request.XAiExtensions is not null)
        {
            if (!string.IsNullOrEmpty(request.XAiExtensions.User))
            {
                xaiRequest.User = request.XAiExtensions.User;
            }
            
            if (!string.IsNullOrEmpty(request.XAiExtensions.UploadUrl))
            {
                xaiRequest.Output = new VendorXAiVideoOutput
                {
                    UploadUrl = request.XAiExtensions.UploadUrl
                };
            }
        }
        
        string json = JsonConvert.SerializeObject(xaiRequest, SerializerSettings);
        
        // xAI uses /v1/videos/edits endpoint
        string url = endpoint.GetUrl(provider, "/edits");
        
        HttpCallResult<VendorXAiVideoCreateResponse> result = await endpoint.HttpPost<VendorXAiVideoCreateResponse>(
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
            Id = result.Data.RequestId,
            Status = VideoJobStatus.Queued, // xAI returns "pending" which maps to Queued
            Model = request.Model?.Name,
            Prompt = prompt,
            SourceProvider = LLmProviders.XAi
        };
        
        return new HttpCallResult<VideoJob>(result.Code, result.Response, job, true, result.Request);
    }
    
    /// <summary>
    /// Retrieves the status of a video job.
    /// </summary>
    public static async Task<HttpCallResult<VideoJob>> Get(
        string requestId, 
        IEndpointProvider provider, 
        EndpointBase endpoint, 
        CancellationToken cancellationToken)
    {
        // xAI uses /v1/videos/{request_id} endpoint
        string url = endpoint.GetUrl(provider, $"/{requestId}");
        
        HttpCallResult<VendorXAiVideoStatusResponse> result = await endpoint.HttpGet<VendorXAiVideoStatusResponse>(
            provider, 
            CapabilityEndpoints.Videos, 
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
            Id = requestId,
            SourceProvider = LLmProviders.XAi
        };
        
        // Map xAI status to harmonized VideoJobStatus
        // xAI uses "pending" (waiting) and "done" (completed)
        // Also handles direct format where video is returned at top level without status
        if (result.Data.IsCompleted)
        {
            job.Status = VideoJobStatus.Completed;
        }
        else if (result.Data.IsPending)
        {
            job.Status = VideoJobStatus.Queued;
        }
        else
        {
            job.Status = VideoJobStatus.Unknown;
        }
        
        // Get video details from either wrapped or direct format
        job.Model = result.Data.GetModel();
        VendorXAiVideoResultData? video = result.Data.GetVideo();
        
        if (video is not null)
        {
            job.Seconds = video.Duration.ToString();
            job.VideoUri = video.Url;
            
            // If moderation failed, mark as failed
            if (!video.RespectModeration)
            {
                job.Status = VideoJobStatus.Failed;
                job.Error = new VideoJobError
                {
                    Message = "Video was blocked due to moderation rules."
                };
            }
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
        
        // For xAI, the video URL is returned directly - we need to fetch it
        return await endpoint.HttpGetStream(
            provider,
            CapabilityEndpoints.None,
            videoUri,
            ct: cancellationToken
        ).ConfigureAwait(false);
    }
}

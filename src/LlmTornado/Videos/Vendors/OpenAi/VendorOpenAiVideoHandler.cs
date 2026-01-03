using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using LlmTornado.Code;
using LlmTornado.Common;

namespace LlmTornado.Videos.Vendors.OpenAi;

/// <summary>
/// Handles all video operations for OpenAI.
/// </summary>
internal static class VendorOpenAiVideoHandler
{
    /// <summary>
    /// Creates a new video generation job.
    /// </summary>
    public static async Task<HttpCallResult<VideoJob>> Create(
        VideoGenerationRequest request, 
        IEndpointProvider provider, 
        EndpointBase endpoint, 
        CancellationToken cancellationToken)
    {
        MultipartFormDataContent content = VendorOpenAiVideoRequest.Serialize(request);
        
        HttpCallResult<VideoJob> result = await endpoint.HttpPost<VideoJob>(
            provider, 
            CapabilityEndpoints.Videos, 
            postData: content,
            ct: cancellationToken
        ).ConfigureAwait(false);
        
        SetSourceProvider(result);
        return result;
    }
    
    /// <summary>
    /// Retrieves the status of a video job.
    /// </summary>
    public static async Task<HttpCallResult<VideoJob>> Get(
        string videoId, 
        IEndpointProvider provider, 
        EndpointBase endpoint, 
        CancellationToken cancellationToken)
    {
        string url = endpoint.GetUrl(provider, $"/{videoId}");
        
        HttpCallResult<VideoJob> result = await endpoint.HttpGet<VideoJob>(
            provider, 
            CapabilityEndpoints.Videos, 
            url,
            ct: cancellationToken
        ).ConfigureAwait(false);
        
        SetSourceProvider(result);
        return result;
    }
    
    /// <summary>
    /// Lists videos with optional pagination.
    /// </summary>
    public static async Task<HttpCallResult<ListResponse<VideoJob>>> List(
        ListQuery? query,
        IEndpointProvider provider, 
        EndpointBase endpoint, 
        CancellationToken cancellationToken)
    {
        string url = endpoint.GetUrl(provider);
        
        HttpCallResult<ListResponse<VideoJob>> result = await endpoint.HttpGet<ListResponse<VideoJob>>(
            provider, 
            CapabilityEndpoints.Videos,
            url,
            queryParams: query?.ToQueryParams(provider),
            ct: cancellationToken
        ).ConfigureAwait(false);
        
        // Set source provider for all items in the list
        if (result.Data?.Items is not null)
        {
            foreach (VideoJob job in result.Data.Items)
            {
                job.SourceProvider = LLmProviders.OpenAi;
            }
        }
        
        return result;
    }
    
    /// <summary>
    /// Deletes a video job.
    /// </summary>
    public static async Task<HttpCallResult<VideoJob>> Delete(
        string videoId,
        IEndpointProvider provider, 
        EndpointBase endpoint, 
        CancellationToken cancellationToken)
    {
        string url = endpoint.GetUrl(provider, $"/{videoId}");
        
        HttpCallResult<VideoJob> result = await endpoint.HttpDelete<VideoJob>(
            provider, 
            CapabilityEndpoints.Videos, 
            url,
            ct: cancellationToken
        ).ConfigureAwait(false);
        
        SetSourceProvider(result);
        return result;
    }
    
    /// <summary>
    /// Downloads video content.
    /// </summary>
    public static async Task<StreamResponse?> GetContent(
        string videoId,
        VideoContentVariant? variant,
        IEndpointProvider provider, 
        EndpointBase endpoint, 
        CancellationToken cancellationToken)
    {
        Dictionary<string, object>? queryParams = null;
        
        if (variant is not null && variant != VideoContentVariant.Video)
        {
            queryParams = new Dictionary<string, object>
            {
                ["variant"] = variant.Value.ToString().ToLowerInvariant()
            };
        }
        
        string url = endpoint.GetUrl(provider, $"/{videoId}/content");
        
        return await endpoint.HttpGetStream(
            provider, 
            CapabilityEndpoints.Videos, 
            url,
            queryParams: queryParams,
            ct: cancellationToken
        ).ConfigureAwait(false);
    }
    
    /// <summary>
    /// Creates a remix of a completed video.
    /// </summary>
    public static async Task<HttpCallResult<VideoJob>> Remix(
        string videoId,
        string prompt,
        IEndpointProvider provider, 
        EndpointBase endpoint, 
        CancellationToken cancellationToken)
    {
        string url = endpoint.GetUrl(provider, $"/{videoId}/remix");
        
        object requestBody = new { prompt };
        
        HttpCallResult<VideoJob> result = await endpoint.HttpPost<VideoJob>(
            provider, 
            CapabilityEndpoints.Videos, 
            url,
            postData: requestBody,
            ct: cancellationToken
        ).ConfigureAwait(false);
        
        SetSourceProvider(result);
        return result;
    }
    
    /// <summary>
    /// Sets the SourceProvider on the VideoJob result.
    /// </summary>
    private static void SetSourceProvider(HttpCallResult<VideoJob> result)
    {
        if (result.Data is not null)
        {
            result.Data.SourceProvider = LLmProviders.OpenAi;
        }
    }
}

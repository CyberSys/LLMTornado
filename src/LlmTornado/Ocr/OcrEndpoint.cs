using System.Threading;
using System.Threading.Tasks;
using LlmTornado.Code;
using LlmTornado.Common;
using LlmTornado.Ocr.Models;

namespace LlmTornado.Ocr;

/// <summary>
/// OCR endpoint for processing documents to extract text, layout, and other information.
/// </summary>
public class OcrEndpoint : EndpointBase
{
    /// <summary>
    /// Constructor of the api endpoint. Rather than instantiating this yourself, access it through an instance of
    /// <see cref="TornadoApi" /> as <see cref="TornadoApi.Ocr" />.
    /// </summary>
    /// <param name="api"></param>
    internal OcrEndpoint(TornadoApi api) : base(api)
    {
    }

    /// <summary>
    /// The name of the endpoint, which is the final path segment in the API URL.
    /// </summary>
    protected override CapabilityEndpoints Endpoint => CapabilityEndpoints.Ocr;

    /// <summary>
    /// Process a document to extract text, layout, and other information.
    /// </summary>
    /// <param name="request">The request to send to the API.</param>
    /// <returns>Asynchronously returns the OCR result.</returns>
    public async Task<OcrResult?> Process(OcrRequest request)
    {
        HttpCallResult<OcrResult> result = await ProcessSafe(request).ConfigureAwait(false);

        if (result.Exception is not null)
        {
            throw result.Exception;
        }

        return result.Data;
    }

    /// <summary>
    /// Process a document to extract text, layout, and other information.
    /// </summary>
    /// <param name="model">The model to use for OCR.</param>
    /// <param name="document">The document to run OCR on.</param>
    /// <returns>Asynchronously returns the OCR result.</returns>
    public async Task<OcrResult?> Process(OcrModel model, OcrDocumentInput document)
    {
        return await Process(new OcrRequest(model, document)).ConfigureAwait(false);
    }

    /// <summary>
    /// Process a document to extract text, layout, and other information. This method doesn't throw exceptions (even if the network layer fails).
    /// </summary>
    /// <param name="request">The request to send to the API.</param>
    /// <returns>Asynchronously returns the OCR result.</returns>
    public async Task<HttpCallResult<OcrResult>> ProcessSafe(OcrRequest request)
    {
        IEndpointProvider provider = Api.GetProvider(request.Model);
        TornadoRequestContent requestBody = request.Serialize(provider);
        HttpCallResult<OcrResult> result = await HttpPost<OcrResult>(provider, Endpoint, requestBody.Url, requestBody.Body, request.Model, request, CancellationToken.None).ConfigureAwait(false);
        
        if (result.Data is not null)
        {
            result.Data.RawResponse = result.Response;
            result.Data.Request = result.Request?.HttpRequest;
        }
        
        return result;
    }
}

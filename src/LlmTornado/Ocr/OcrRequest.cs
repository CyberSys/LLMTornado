using System.Collections.Generic;
using LlmTornado.Chat;
using LlmTornado.Code;
using LlmTornado.Ocr.Models;
using Newtonsoft.Json;

namespace LlmTornado.Ocr;

/// <summary>
/// A request for the OCR API.
/// </summary>
public class OcrRequest : ISerializableRequest
{
    /// <summary>
    /// Creates a new, empty <see cref="OcrRequest"/>
    /// </summary>
    public OcrRequest()
    {
    }

    /// <summary>
    /// Creates a new request for the OCR API.
    /// </summary>
    /// <param name="model">The model to use for OCR.</param>
    /// <param name="document">The document to run OCR on.</param>
    public OcrRequest(OcrModel model, OcrDocumentInput document)
    {
        Model = model;
        Document = document;
    }

    /// <summary>
    /// Create a new OCR request using the data from the input request.
    /// </summary>
    /// <param name="basedOn"></param>
    public OcrRequest(OcrRequest? basedOn)
    {
        if (basedOn is null)
        {
            return;
        }

        CopyData(basedOn);
    }

    private void CopyData(OcrRequest basedOn)
    {
        Model = basedOn.Model;
        Document = basedOn.Document;
        Id = basedOn.Id;
        Pages = basedOn.Pages;
        IncludeImageBase64 = basedOn.IncludeImageBase64;
        ImageLimit = basedOn.ImageLimit;
        ImageMinSize = basedOn.ImageMinSize;
        ExtractHeader = basedOn.ExtractHeader;
        ExtractFooter = basedOn.ExtractFooter;
        TableFormat = basedOn.TableFormat;
        BboxAnnotationFormat = basedOn.BboxAnnotationFormat;
        DocumentAnnotationFormat = basedOn.DocumentAnnotationFormat;
    }

    /// <summary>
    /// The model to use for OCR (e.g., "mistral-ocr-latest").
    /// </summary>
    [JsonProperty("model")]
    [JsonConverter(typeof(IModelConverter))]
    public OcrModel Model { get; set; }

    /// <summary>
    /// The document to run OCR on.
    /// </summary>
    [JsonProperty("document")]
    public OcrDocumentInput Document { get; set; }

    /// <summary>
    /// Optional client-side ID for the request.
    /// </summary>
    [JsonProperty("id")]
    public string? Id { get; set; }

    /// <summary>
    /// Specific pages user wants to process. Can be a list of page numbers (0-indexed).
    /// </summary>
    [JsonProperty("pages")]
    public List<int>? Pages { get; set; }

    /// <summary>
    /// Include image base64 strings in the response.
    /// </summary>
    [JsonProperty("include_image_base64")]
    public bool? IncludeImageBase64 { get; set; }

    /// <summary>
    /// Max images to extract.
    /// </summary>
    [JsonProperty("image_limit")]
    public int? ImageLimit { get; set; }

    /// <summary>
    /// Minimum height and width of image to extract.
    /// </summary>
    [JsonProperty("image_min_size")]
    public int? ImageMinSize { get; set; }

    /// <summary>
    /// Whether to extract headers.
    /// </summary>
    [JsonProperty("extract_header")]
    public bool? ExtractHeader { get; set; }

    /// <summary>
    /// Whether to extract footers.
    /// </summary>
    [JsonProperty("extract_footer")]
    public bool? ExtractFooter { get; set; }

    /// <summary>
    /// Format for tables.
    /// </summary>
    [JsonProperty("table_format")]
    public OcrTableFormat? TableFormat { get; set; }

    /// <summary>
    /// Specify the format that the model must output for bounding box annotations.
    /// </summary>
    [JsonProperty("bbox_annotation_format")]
    public ChatRequestResponseFormats? BboxAnnotationFormat { get; set; }

    /// <summary>
    /// Specify the format that the model must output for document annotations.
    /// </summary>
    [JsonProperty("document_annotation_format")]
    public ChatRequestResponseFormats? DocumentAnnotationFormat { get; set; }

    [JsonIgnore]
    internal string? UrlOverride { get; set; }

    internal void OverrideUrl(string url)
    {
        UrlOverride = url;
    }

    /// <summary>
    /// Serializes the request.
    /// </summary>
    public TornadoRequestContent Serialize(IEndpointProvider provider, RequestSerializeOptions options)
    {
        return SerializeInternal(provider, options);
    }

    /// <summary>
    /// Serializes the request.
    /// </summary>
    public TornadoRequestContent Serialize(IEndpointProvider provider)
    {
        return SerializeInternal(provider, null);
    }

    /// <summary>
    /// Serializes the request.
    /// </summary>
    internal TornadoRequestContent SerializeInternal(IEndpointProvider provider, RequestSerializeOptions? options)
    {
        return new TornadoRequestContent(this.ToJson(options?.Pretty ?? false), Model, UrlOverride ?? EndpointBase.BuildRequestUrl(null, provider, CapabilityEndpoints.Ocr, Model), provider, CapabilityEndpoints.Ocr);
    }
}

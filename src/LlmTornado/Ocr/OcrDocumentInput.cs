using System;
using LlmTornado.Files;
using Newtonsoft.Json;

namespace LlmTornado.Ocr;

/// <summary>
/// Represents a document input for OCR processing.
/// </summary>
public class OcrDocumentInput
{
    /// <summary>
    /// The type of document input.
    /// </summary>
    [JsonProperty("type")]
    public OcrDocumentType Type { get; set; }
    
    /// <summary>
    /// File ID for uploaded files. Used when Type is File.
    /// </summary>
    [JsonProperty("file_id")]
    public string? FileId { get; set; }
    
    /// <summary>
    /// Document URL for public documents. Used when Type is DocumentUrl.
    /// </summary>
    [JsonProperty("document_url")]
    public string? DocumentUrl { get; set; }
    
    /// <summary>
    /// Image URL configuration for images. Used when Type is ImageUrl.
    /// </summary>
    [JsonProperty("image_url")]
    public OcrImageUrlInput? ImageUrl { get; set; }

    /// <summary>
    /// Creates a document input from a file ID (alias for FromFileId).
    /// </summary>
    /// <param name="fileId">The file ID from upload</param>
    public static OcrDocumentInput FromFile(string fileId)
    {
        return FromFileId(fileId);
    }

    /// <summary>
    /// Creates a document input from a file ID (uploaded to provider's file storage).
    /// </summary>
    /// <param name="fileId">The file ID from upload</param>
    public static OcrDocumentInput FromFileId(string fileId)
    {
        return new OcrDocumentInput
        {
            Type = OcrDocumentType.File,
            FileId = fileId
        };
    }
    
    /// <summary>
    /// Creates a document input from a file.
    /// </summary>
    /// <param name="file">The uploaded file</param>
    public static OcrDocumentInput FromFile(TornadoFile file)
    {
        return new OcrDocumentInput
        {
            Type = OcrDocumentType.File,
            FileId = file.Id
        };
    }

    /// <summary>
    /// Creates a document input from a public document URL (PDF, DOCX, etc.).
    /// </summary>
    /// <param name="url">Public URL to the document</param>
    public static OcrDocumentInput FromDocumentUrl(string url)
    {
        return new OcrDocumentInput
        {
            Type = OcrDocumentType.DocumentUrl,
            DocumentUrl = url
        };
    }

    /// <summary>
    /// Creates a document input from an image URL.
    /// </summary>
    /// <param name="url">Public URL to the image</param>
    /// <param name="detail">Optional detail level for processing</param>
    public static OcrDocumentInput FromImageUrl(string url, OcrImageDetail? detail = null)
    {
        return new OcrDocumentInput
        {
            Type = OcrDocumentType.ImageUrl,
            ImageUrl = new OcrImageUrlInput(url, detail)
        };
    }

    /// <summary>
    /// Creates a document input from image bytes.
    /// </summary>
    /// <param name="imageBytes">Image file bytes</param>
    /// <param name="mimeType">MIME type of the image (e.g., "image/jpeg", "image/png")</param>
    /// <param name="detail">Optional detail level for processing</param>
    public static OcrDocumentInput FromImageBytes(byte[] imageBytes, string mimeType, OcrImageDetail? detail = null)
    {
        string base64 = Convert.ToBase64String(imageBytes);
        return new OcrDocumentInput
        {
            Type = OcrDocumentType.ImageUrl,
            ImageUrl = new OcrImageUrlInput($"data:{mimeType};base64,{base64}", detail)
        };
    }
}

/// <summary>
/// Image URL input configuration for OCR.
/// </summary>
public class OcrImageUrlInput
{
    /// <summary>
    /// Creates an image URL input.
    /// </summary>
    /// <param name="url">The URL (public or data URL)</param>
    /// <param name="detail">Optional detail level</param>
    public OcrImageUrlInput(string url, OcrImageDetail? detail = null)
    {
        Url = url;
        Detail = detail;
    }

    /// <summary>
    /// Default constructor for serialization.
    /// </summary>
    public OcrImageUrlInput()
    {
    }

    /// <summary>
    /// The URL of the image (public URL or data URL with base64).
    /// </summary>
    [JsonProperty("url")]
    public string Url { get; set; }
    
    /// <summary>
    /// Optional detail level for image processing.
    /// </summary>
    [JsonProperty("detail", NullValueHandling = NullValueHandling.Ignore)]
    public OcrImageDetail? Detail { get; set; }
}

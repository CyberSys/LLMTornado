using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace LlmTornado.Ocr;

/// <summary>
/// Table formatting options for OCR output.
/// </summary>
[JsonConverter(typeof(StringEnumConverter))]
public enum OcrTableFormat
{
    /// <summary>
    /// Return tables as markdown.
    /// </summary>
    [EnumMember(Value = "markdown")]
    Markdown,
    
    /// <summary>
    /// Return tables as HTML.
    /// </summary>
    [EnumMember(Value = "html")]
    Html
}

/// <summary>
/// Document input type for OCR.
/// </summary>
[JsonConverter(typeof(StringEnumConverter))]
public enum OcrDocumentType
{
    /// <summary>
    /// File uploaded to provider's file storage.
    /// </summary>
    [EnumMember(Value = "file")]
    File,
    
    /// <summary>
    /// Public URL to a document (PDF, DOCX, etc.).
    /// </summary>
    [EnumMember(Value = "document_url")]
    DocumentUrl,
    
    /// <summary>
    /// Public URL or data URL to an image.
    /// </summary>
    [EnumMember(Value = "image_url")]
    ImageUrl
}

/// <summary>
/// Image detail level for OCR processing.
/// </summary>
[JsonConverter(typeof(StringEnumConverter))]
public enum OcrImageDetail
{
    /// <summary>
    /// Automatically decides the detail level.
    /// </summary>
    [EnumMember(Value = "auto")]
    Auto,
    
    /// <summary>
    /// High detail - images will be tiled for better accuracy.
    /// </summary>
    [EnumMember(Value = "high")]
    High,
    
    /// <summary>
    /// Low detail - images passed as single tile, faster processing.
    /// </summary>
    [EnumMember(Value = "low")]
    Low
}

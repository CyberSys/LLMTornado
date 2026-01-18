using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace LlmTornado.Files;

/// <summary>
/// Sample type of the file.
/// </summary>
[JsonConverter(typeof(StringEnumConverter))]
public enum TornadoFileSampleType
{
    /// <summary>
    /// Pretraining data.
    /// </summary>
    [EnumMember(Value = "pretrain")]
    Pretrain,
    
    /// <summary>
    /// Instruction data.
    /// </summary>
    [EnumMember(Value = "instruct")]
    Instruct,
    
    /// <summary>
    /// Batch request.
    /// </summary>
    [EnumMember(Value = "batch_request")]
    BatchRequest,
    
    /// <summary>
    /// Batch result.
    /// </summary>
    [EnumMember(Value = "batch_result")]
    BatchResult,
    
    /// <summary>
    /// Batch error.
    /// </summary>
    [EnumMember(Value = "batch_error")]
    BatchError,
    
    /// <summary>
    /// OCR input.
    /// </summary>
    [EnumMember(Value = "ocr_input")]
    OcrInput
}

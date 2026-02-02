namespace LlmTornado.Images.Vendors.XAi;

/// <summary>
/// Extensions to image generation request for xAI.
/// </summary>
public class ImageGenerationRequestXAiExtensions
{
    /// <summary>
    /// Aspect ratio of the generated image. Defaults to 1:1.
    /// Supported values: Square (1:1), Portrait3x4, Landscape4x3, Portrait9x16, Landscape16x9, 
    /// Portrait2x3, Landscape3x2, Portrait9x19_5, Landscape19_5x9, Portrait9x20, Landscape20x9, 
    /// Portrait1x2, Landscape2x1, Auto.
    /// </summary>
    public ImageAspectRatio? AspectRatio { get; set; }
    
    /// <summary>
    /// Resolution of the generated image. Defaults to 1k. Currently, only 1k is supported. Support for 2k will be available shortly.
    /// </summary>
    public ImageResolution? Resolution { get; set; }
}

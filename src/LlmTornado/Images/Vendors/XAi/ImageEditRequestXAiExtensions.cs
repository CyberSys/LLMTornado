namespace LlmTornado.Images.Vendors.XAi;

/// <summary>
/// Extensions to image edit request for xAI.
/// </summary>
public class ImageEditRequestXAiExtensions
{
    /// <summary>
    /// Resolution of the generated image. Defaults to 1k. Aspect ratio is automatically detected from the input image.
    /// </summary>
    public ImageResolution? Resolution { get; set; }
    
    /// <summary>
    /// Specifies the detail level of the input image. Optional.
    /// </summary>
    public ImageDetail? ImageDetail { get; set; }
    
    /// <summary>
    /// Specifies the detail level of the mask image. Optional.
    /// </summary>
    public ImageDetail? MaskDetail { get; set; }
}

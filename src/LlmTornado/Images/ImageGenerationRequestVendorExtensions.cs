using LlmTornado.Images.Vendors.Google;
using LlmTornado.Images.Vendors.XAi;

namespace LlmTornado.Images;

/// <summary>
///     Image generation request features supported only by a single/few providers with no shared equivalent.
/// </summary>
public class ImageGenerationRequestVendorExtensions
{
    /// <summary>
    ///     Google extensions.
    /// </summary>
    public ImageGenerationRequestGoogleExtensions? Google { get; set; }
    
    /// <summary>
    ///     xAI extensions.
    /// </summary>
    public ImageGenerationRequestXAiExtensions? XAi { get; set; }

    /// <summary>
    ///     Empty extensions.
    /// </summary>
    public ImageGenerationRequestVendorExtensions()
    {
        
    }
    
    /// <summary>
    ///     Google extensions.
    /// </summary>
    /// <param name="googleExtensions"></param>
    public ImageGenerationRequestVendorExtensions(ImageGenerationRequestGoogleExtensions googleExtensions)
    {
        Google = googleExtensions;
    }
    
    /// <summary>
    ///     xAI extensions.
    /// </summary>
    /// <param name="xAiExtensions"></param>
    public ImageGenerationRequestVendorExtensions(ImageGenerationRequestXAiExtensions xAiExtensions)
    {
        XAi = xAiExtensions;
    }
}
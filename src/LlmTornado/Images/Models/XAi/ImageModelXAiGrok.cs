using System;
using System.Collections.Generic;
using LlmTornado.Code;
using LlmTornado.Code.Models;

namespace LlmTornado.Images.Models.XAi;


/// <summary>
/// Grok class models from xAI.
/// </summary>
public class ImageModelXAiGrok : IVendorModelClassProvider
{
    /// <summary>
    /// Our latest image generation model, capable of creating high-quality, detailed images from text prompts with enhanced creativity and precision.
    /// </summary>
    public static readonly ImageModel ModelV2241212 = new ImageModel("grok-2-image-1212", LLmProviders.XAi, [ "grok-2-image", "grok-2-image-latest" ]);

    /// <summary>
    /// <inheritdoc cref="ModelV2241212"/>
    /// </summary>
    public readonly ImageModel V2241212 = ModelV2241212;
    
    /// <summary>
    /// Grok Imagine image generation model, capable of creating and editing high-quality images from text prompts.
    /// Supports aspect ratios, resolutions, and image editing with masks.
    /// </summary>
    public static readonly ImageModel ModelImagine = new ImageModel("grok-imagine-image", LLmProviders.XAi);
    
    /// <summary>
    /// <inheritdoc cref="ModelImagine"/>
    /// </summary>
    public readonly ImageModel Imagine = ModelImagine;
    
    /// <summary>
    /// All known Grok models from xAI.
    /// </summary>
    public static List<IModel> ModelsAll => LazyModelsAll.Value;
    
    private static readonly Lazy<List<IModel>> LazyModelsAll = new Lazy<List<IModel>>(() => [
        ModelV2241212,
        ModelImagine
    ]);

    /// <summary>
    /// <inheritdoc cref="ModelsAll"/>
    /// </summary>
    public List<IModel> AllModels => ModelsAll;
    
    internal ImageModelXAiGrok()
    {
        
    }
}
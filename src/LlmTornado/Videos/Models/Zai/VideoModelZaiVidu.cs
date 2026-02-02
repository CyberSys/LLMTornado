using System;
using System.Collections.Generic;
using LlmTornado.Code;
using LlmTornado.Code.Models;

namespace LlmTornado.Videos.Models.Zai;

/// <summary>
/// Z.AI Vidu video models - high-performance video models with high consistency and dynamism.
/// </summary>
public class VideoModelZaiVidu : BaseVendorModelProvider
{
    /// <inheritdoc cref="BaseVendorModelProvider.Provider"/>
    public override LLmProviders Provider => LLmProviders.Zai;

    public override List<IModel> AllModels => ModelsAll;

    /// <summary>
    /// Vidu Q1 Text-to-Video - Text-to-video generation with style control (general/anime),
    /// aspect ratios (16:9, 9:16, 1:1), movement amplitude control, and 5s duration.
    /// Resolution: 1920x1080.
    /// </summary>
    public static readonly VideoModel ModelViduQ1Text = new VideoModel("viduq1-text", LLmProviders.Zai);
    
    /// <summary>
    /// <inheritdoc cref="ModelViduQ1Text"/>
    /// </summary>
    public readonly VideoModel Q1Text = ModelViduQ1Text;

    /// <summary>
    /// Vidu Q1 Image-to-Video - Uses an input image as the first frame to generate video.
    /// Supports movement amplitude control, audio, and 5s duration. Resolution: 1920x1080.
    /// </summary>
    public static readonly VideoModel ModelViduQ1Image = new VideoModel("viduq1-image", LLmProviders.Zai);
    
    /// <summary>
    /// <inheritdoc cref="ModelViduQ1Image"/>
    /// </summary>
    public readonly VideoModel Q1Image = ModelViduQ1Image;

    /// <summary>
    /// Vidu Q1 Start-End Frame - Uses two images (first and last frame) to generate video.
    /// Supports movement amplitude control, audio, and 5s duration. Resolution: 1920x1080.
    /// </summary>
    public static readonly VideoModel ModelViduQ1StartEnd = new VideoModel("viduq1-start-end", LLmProviders.Zai);
    
    /// <summary>
    /// <inheritdoc cref="ModelViduQ1StartEnd"/>
    /// </summary>
    public readonly VideoModel Q1StartEnd = ModelViduQ1StartEnd;

    /// <summary>
    /// Vidu 2 Image-to-Video - Uses an input image as the first frame to generate video.
    /// Supports movement amplitude control, audio, and 4s duration. Resolution: 1280x720.
    /// </summary>
    public static readonly VideoModel ModelVidu2Image = new VideoModel("vidu2-image", LLmProviders.Zai);
    
    /// <summary>
    /// <inheritdoc cref="ModelVidu2Image"/>
    /// </summary>
    public readonly VideoModel V2Image = ModelVidu2Image;

    /// <summary>
    /// Vidu 2 Start-End Frame - Uses two images (first and last frame) to generate video.
    /// Supports movement amplitude control, audio, and 4s duration. Resolution: 1280x720.
    /// </summary>
    public static readonly VideoModel ModelVidu2StartEnd = new VideoModel("vidu2-start-end", LLmProviders.Zai);
    
    /// <summary>
    /// <inheritdoc cref="ModelVidu2StartEnd"/>
    /// </summary>
    public readonly VideoModel V2StartEnd = ModelVidu2StartEnd;

    /// <summary>
    /// Vidu 2 Reference - Uses 1-3 reference images to generate video with consistent subjects.
    /// Supports aspect ratios (16:9, 9:16, 1:1), movement amplitude control, audio, and 4s duration.
    /// Resolution: 1280x720.
    /// </summary>
    public static readonly VideoModel ModelVidu2Reference = new VideoModel("vidu2-reference", LLmProviders.Zai);
    
    /// <summary>
    /// <inheritdoc cref="ModelVidu2Reference"/>
    /// </summary>
    public readonly VideoModel V2Reference = ModelVidu2Reference;

    /// <summary>
    /// All known Vidu video models.
    /// </summary>
    public static List<IModel> ModelsAll => LazyModelsAll.Value;

    private static readonly Lazy<List<IModel>> LazyModelsAll = new Lazy<List<IModel>>(() => [
        ModelViduQ1Text,
        ModelViduQ1Image,
        ModelViduQ1StartEnd,
        ModelVidu2Image,
        ModelVidu2StartEnd,
        ModelVidu2Reference
    ]);
    
    /// <summary>
    /// Checks whether a model is owned by the provider.
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    public override bool OwnsModel(string model)
    {
        return AllModelsMap.Contains(model);
    }

    /// <summary>
    /// Map of models owned by the provider.
    /// </summary>
    public static HashSet<string> AllModelsMap => LazyAllModelsMap.Value;

    private static readonly Lazy<HashSet<string>> LazyAllModelsMap = new Lazy<HashSet<string>>(() =>
    {
        HashSet<string> map = [];
        ModelsAll.ForEach(x => { map.Add(x.Name); });
        return map;
    });
    
    internal VideoModelZaiVidu()
    {
        
    }
}

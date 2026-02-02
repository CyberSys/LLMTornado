using System;
using System.Collections.Generic;
using LlmTornado.Code;
using LlmTornado.Code.Models;

namespace LlmTornado.Videos.Models.XAi;

/// <summary>
/// xAI Grok video models.
/// </summary>
public class VideoModelXAiGrok : BaseVendorModelProvider
{
    /// <inheritdoc cref="BaseVendorModelProvider.Provider"/>
    public override LLmProviders Provider => LLmProviders.XAi;

    public override List<IModel> AllModels => ModelsAll;

    /// <summary>
    /// Grok Imagine Video - xAI's video generation model capable of generating videos from text prompts and images.
    /// Supports durations from 1-15 seconds, multiple aspect ratios (1:1, 16:9, 9:16, 4:3, 3:4, 3:2, 2:3), 
    /// and resolutions (480p, 720p).
    /// </summary>
    public static readonly VideoModel ModelGrokImagineVideo = new VideoModel("grok-imagine-video", LLmProviders.XAi);
    
    /// <summary>
    /// <inheritdoc cref="ModelGrokImagineVideo"/>
    /// </summary>
    public readonly VideoModel ImagineVideo = ModelGrokImagineVideo;

    /// <summary>
    /// All known Grok video models.
    /// </summary>
    public static List<IModel> ModelsAll => LazyModelsAll.Value;

    private static readonly Lazy<List<IModel>> LazyModelsAll = new Lazy<List<IModel>>(() => [
        ModelGrokImagineVideo
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
    
    internal VideoModelXAiGrok()
    {
        
    }
}

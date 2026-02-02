using System;
using System.Collections.Generic;
using LlmTornado.Code;
using LlmTornado.Code.Models;

namespace LlmTornado.Videos.Models.Zai;

/// <summary>
/// Z.AI CogVideoX video models.
/// </summary>
public class VideoModelZaiCogVideoX : BaseVendorModelProvider
{
    /// <inheritdoc cref="BaseVendorModelProvider.Provider"/>
    public override LLmProviders Provider => LLmProviders.Zai;

    public override List<IModel> AllModels => ModelsAll;

    /// <summary>
    /// CogVideoX-3 - Z.AI's video generation model with powerful text-to-video and image-to-video capabilities.
    /// Supports quality/speed modes, audio generation, durations of 5s and 10s, FPS of 30 or 60,
    /// and resolutions up to 4K (1280x720, 720x1280, 1024x1024, 1920x1080, 1080x1920, 2048x1080, 3840x2160).
    /// Also supports first/last frame mode with two input images.
    /// </summary>
    public static readonly VideoModel ModelCogVideoX3 = new VideoModel("cogvideox-3", LLmProviders.Zai);
    
    /// <summary>
    /// <inheritdoc cref="ModelCogVideoX3"/>
    /// </summary>
    public readonly VideoModel V3 = ModelCogVideoX3;

    /// <summary>
    /// All known CogVideoX video models.
    /// </summary>
    public static List<IModel> ModelsAll => LazyModelsAll.Value;

    private static readonly Lazy<List<IModel>> LazyModelsAll = new Lazy<List<IModel>>(() => [
        ModelCogVideoX3
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
    
    internal VideoModelZaiCogVideoX()
    {
        
    }
}

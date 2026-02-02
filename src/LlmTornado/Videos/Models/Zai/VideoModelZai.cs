using System;
using System.Collections.Generic;
using LlmTornado.Code;
using LlmTornado.Code.Models;

namespace LlmTornado.Videos.Models.Zai;

/// <summary>
/// Known video models from Z.AI.
/// </summary>
public class VideoModelZai : BaseVendorModelProvider
{
    /// <inheritdoc cref="BaseVendorModelProvider.Provider"/>
    public override LLmProviders Provider => LLmProviders.Zai;
    
    /// <summary>
    /// CogVideoX video models - powerful text-to-video and image-to-video with quality/speed modes.
    /// </summary>
    public readonly VideoModelZaiCogVideoX CogVideoX = new VideoModelZaiCogVideoX();
    
    /// <summary>
    /// Vidu video models - high-performance models with text-to-video, image-to-video, 
    /// first/last frame, and reference-based generation.
    /// </summary>
    public readonly VideoModelZaiVidu Vidu = new VideoModelZaiVidu();
    
    /// <summary>
    /// All known video models from Z.AI.
    /// </summary>
    public override List<IModel> AllModels => ModelsAll;
    
    /// <summary>
    /// Checks whether the model is owned by the provider.
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
    
    /// <summary>
    /// <inheritdoc cref="AllModels"/>
    /// </summary>
    public static List<IModel> ModelsAll => LazyModelsAll.Value;

    private static readonly Lazy<List<IModel>> LazyModelsAll = new Lazy<List<IModel>>(() => [
        ..VideoModelZaiCogVideoX.ModelsAll,
        ..VideoModelZaiVidu.ModelsAll
    ]);
    
    internal VideoModelZai()
    {
        
    }
}

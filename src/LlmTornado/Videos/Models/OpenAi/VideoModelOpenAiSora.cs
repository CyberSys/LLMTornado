using System;
using System.Collections.Generic;
using LlmTornado.Code;
using LlmTornado.Code.Models;

namespace LlmTornado.Videos.Models.OpenAi;

/// <summary>
/// OpenAI Sora video models.
/// </summary>
public class VideoModelOpenAiSora : BaseVendorModelProvider
{
    /// <inheritdoc cref="BaseVendorModelProvider.Provider"/>
    public override LLmProviders Provider => LLmProviders.OpenAi;

    public override List<IModel> AllModels => ModelsAll;

    /// <summary>
    /// Sora 2 - designed for speed and flexibility. Ideal for exploration, rapid iteration, and prototypes.
    /// </summary>
    public static readonly VideoModel ModelSora2 = new VideoModel("sora-2", "openai", LLmProviders.OpenAi);
    
    /// <summary>
    /// <inheritdoc cref="ModelSora2"/>
    /// </summary>
    public readonly VideoModel Sora2 = ModelSora2;
    
    /// <summary>
    /// Sora 2 Pro - produces higher quality results. Best for production-quality output and cinematic footage.
    /// </summary>
    public static readonly VideoModel ModelSora2Pro = new VideoModel("sora-2-pro", "openai", LLmProviders.OpenAi);
    
    /// <summary>
    /// <inheritdoc cref="ModelSora2Pro"/>
    /// </summary>
    public readonly VideoModel Sora2Pro = ModelSora2Pro;
    
    /// <summary>
    /// Sora 2 snapshot from 2025-12-08.
    /// </summary>
    public static readonly VideoModel ModelSora2_20251208 = new VideoModel("sora-2-2025-12-08", "openai", LLmProviders.OpenAi);
    
    /// <summary>
    /// <inheritdoc cref="ModelSora2_20251208"/>
    /// </summary>
    public readonly VideoModel Sora2_20251208 = ModelSora2_20251208;
    
    /// <summary>
    /// Sora 2 snapshot from 2025-10-06.
    /// </summary>
    public static readonly VideoModel ModelSora2_20251006 = new VideoModel("sora-2-2025-10-06", "openai", LLmProviders.OpenAi);
    
    /// <summary>
    /// <inheritdoc cref="ModelSora2_20251006"/>
    /// </summary>
    public readonly VideoModel Sora2_20251006 = ModelSora2_20251006;

    /// <summary>
    /// All known Sora models.
    /// </summary>
    public static List<IModel> ModelsAll => LazyModelsAll.Value;

    private static readonly Lazy<List<IModel>> LazyModelsAll = new Lazy<List<IModel>>(() => [
        ModelSora2, ModelSora2Pro, ModelSora2_20251208, ModelSora2_20251006
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
    
    internal VideoModelOpenAiSora()
    {
        
    }
}

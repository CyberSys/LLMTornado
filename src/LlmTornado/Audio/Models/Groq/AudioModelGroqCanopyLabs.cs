using System;
using System.Collections.Generic;
using LlmTornado.Code;
using LlmTornado.Code.Models;

namespace LlmTornado.Audio.Models.Groq;

/// <summary>
/// Canopy Labs Orpheus TTS models hosted by Groq.
/// </summary>
public class AudioModelGroqCanopyLabs : IVendorModelClassProvider
{
    /// <summary>
    /// Orpheus V1 English text-to-speech model with enhanced expressiveness and vocal direction controls.
    /// </summary>
    public static readonly AudioModel ModelOrpheusV1English = new AudioModel("groq-orpheus-v1-english", LLmProviders.Groq)
    {
        ApiName = "canopylabs/orpheus-v1-english"
    };
    
    /// <summary>
    /// <inheritdoc cref="ModelOrpheusV1English"/>
    /// </summary>
    public readonly AudioModel OrpheusV1English = ModelOrpheusV1English;
    
    /// <summary>
    /// Orpheus Arabic Saudi text-to-speech model optimized for Arabic language.
    /// </summary>
    public static readonly AudioModel ModelOrpheusArabicSaudi = new AudioModel("groq-orpheus-arabic-saudi", LLmProviders.Groq)
    {
        ApiName = "canopylabs/orpheus-arabic-saudi"
    };
    
    /// <summary>
    /// <inheritdoc cref="ModelOrpheusArabicSaudi"/>
    /// </summary>
    public readonly AudioModel OrpheusArabicSaudi = ModelOrpheusArabicSaudi;
    
    /// <summary>
    /// All known Canopy Labs Orpheus TTS models from Groq.
    /// </summary>
    public static List<IModel> ModelsAll => LazyModelsAll.Value;

    private static readonly Lazy<List<IModel>> LazyModelsAll = new Lazy<List<IModel>>(() => [
        ModelOrpheusV1English,
        ModelOrpheusArabicSaudi
    ]);

    /// <summary>
    /// <inheritdoc cref="ModelsAll"/>
    /// </summary>
    public List<IModel> AllModels => ModelsAll;

    internal AudioModelGroqCanopyLabs()
    {

    }
}

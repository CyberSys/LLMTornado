using System;
using System.Collections.Generic;
using LlmTornado.Code;
using LlmTornado.Code.Models;

namespace LlmTornado.Audio.Models.Zai;

/// <summary>
/// ASR (Automatic Speech Recognition) models from Z.AI.
/// </summary>
public class AudioModelZaiAsr : IVendorModelClassProvider
{
    /// <summary>
    /// GLM-ASR-2512 model for audio transcription. Supports .wav and .mp3 formats,
    /// files up to 25MB, and audio duration up to 30 seconds. Supports multiple languages
    /// and real-time streaming transcription.
    /// </summary>
    public static readonly AudioModel ModelGlmAsr2512 = new AudioModel("glm-asr-2512", LLmProviders.Zai);
    
    /// <summary>
    /// <inheritdoc cref="ModelGlmAsr2512"/>
    /// </summary>
    public readonly AudioModel GlmAsr2512 = ModelGlmAsr2512;
    
    /// <summary>
    /// All known ASR models from Z.AI.
    /// </summary>
    public static List<IModel> ModelsAll => LazyModelsAll.Value;

    private static readonly Lazy<List<IModel>> LazyModelsAll = new Lazy<List<IModel>>(() => [
        ModelGlmAsr2512
    ]);

    /// <summary>
    /// <inheritdoc cref="ModelsAll"/>
    /// </summary>
    public List<IModel> AllModels => ModelsAll;

    internal AudioModelZaiAsr()
    {

    }
}

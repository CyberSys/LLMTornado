namespace LlmTornado.Common;

/// <summary>
/// Settings applied to outbound HTTP requests made by the API.
/// </summary>
public class TornadoRequestSettings
{
    /// <summary>
    /// Custom User-Agent header to use for requests. When set, overrides the default User-Agent.
    /// </summary>
    public string? UserAgent { get; set; }
}

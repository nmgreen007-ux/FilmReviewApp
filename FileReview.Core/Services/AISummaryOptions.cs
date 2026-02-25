namespace FileReview.Core.Services;

/// <summary>
/// Configuration options for Azure OpenAI AI summary generation
/// </summary>
public class AISummaryOptions
{
    /// <summary>
    /// Azure OpenAI endpoint URL
    /// </summary>
    public string Endpoint { get; set; } = string.Empty;

    /// <summary>
    /// Azure OpenAI API key
    /// </summary>
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>
    /// Deployment ID (model name) for the OpenAI deployment
    /// </summary>
    public string DeploymentId { get; set; } = "gpt-35-turbo";

    /// <summary>
    /// Temperature for generation (0.0 to 2.0) - lower = more deterministic
    /// </summary>
    public float Temperature { get; set; } = 0.7f;

    /// <summary>
    /// Maximum tokens for the generated summary
    /// </summary>
    public int MaxTokens { get; set; } = 150;

    /// <summary>
    /// Whether to cache AI summaries in memory
    /// </summary>
    public bool EnableCaching { get; set; } = true;
}

using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace FilmReview.Core.Services;

/// <summary>
/// Service for generating AI-powered summaries using Azure OpenAI REST API
/// </summary>
public class AISummaryService : IAISummaryService
{
    private readonly ILogger<AISummaryService> _logger;
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;

    public AISummaryService(
        ILogger<AISummaryService> logger,
        IConfiguration configuration,
        HttpClient httpClient)
    {
        _logger = logger;
        _configuration = configuration;
        _httpClient = httpClient;
    }

    /// <summary>
    /// Generate an AI summary for a film using Azure OpenAI REST API
    /// </summary>
    public async Task<string?> GenerateFilmSummaryAsync(string filmTitle, string reviews)
    {
        try
        {
            // Get Azure OpenAI configuration
            var endpoint = _configuration["AzureOpenAI:Endpoint"];
            var apiKey = _configuration["AzureOpenAI:ApiKey"];
            var deploymentId = _configuration["AzureOpenAI:DeploymentId"] ?? "gpt-5-nano";

            if (string.IsNullOrEmpty(endpoint) || string.IsNullOrEmpty(apiKey))
            {
                _logger.LogWarning("Azure OpenAI configuration is not set. AI summary generation skipped.");
                return null;
            }

            // Construct the prompt
            var prompt = $@"Summarize what reviewers are saying about the film '{filmTitle}' based on these review comments:

{reviews}

Provide a 1-2 sentence summary of the overall sentiment and key themes mentioned in the reviews.";

            // Build the request URL
            var url = $"{endpoint.TrimEnd('/')}/openai/deployments/{deploymentId}/chat/completions?api-version=2025-01-01-preview";

            // Create the request body
            var requestBody = new
            {
                messages = new[]
                {
                    new { role = "system", content = "You are a helpful assistant that writes engaging film summaries." },
                    new { role = "user", content = prompt }
                },
                max_completion_tokens = 2000,
                model = deploymentId
            };

            var jsonContent = new StringContent(
                JsonSerializer.Serialize(requestBody),
                Encoding.UTF8,
                "application/json");

            // Create the request with API key header
            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = jsonContent
            };
            request.Headers.Add("Authorization", $"Bearer {apiKey}");

            // Make the request
            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogWarning($"Azure OpenAI API returned status {response.StatusCode}. Error: {errorContent}");
                return null;
            }

            // Parse the response
            var responseContent = await response.Content.ReadAsStringAsync();
            _logger.LogInformation($"Azure OpenAI Response: {responseContent}");
            
            using var doc = JsonDocument.Parse(responseContent);
            var root = doc.RootElement;

            if (root.TryGetProperty("choices", out var choices) && 
                choices.GetArrayLength() > 0 &&
                choices[0].TryGetProperty("message", out var message) &&
                message.TryGetProperty("content", out var content))
            {
                var summary = content.GetString();
                _logger.LogInformation($"Generated AI summary for film: {filmTitle}");
                return summary;
            }

            _logger.LogWarning("Unexpected response format from Azure OpenAI API");
            _logger.LogWarning($"Response root properties: {string.Join(", ", root.EnumerateObject().Select(p => p.Name))}");
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error generating AI summary for film: {filmTitle}");
            return null;
        }
    }
}



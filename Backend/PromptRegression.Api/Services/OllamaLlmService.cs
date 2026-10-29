using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace PromptRegression.Api.Services;

public class OllamaLlmService(
    HttpClient httpClient) : ILlmService
{
    public async Task<string> GenerateAsync(
        string prompt,
        CancellationToken cancellationToken = default)
    {
        var request = new OllamaRequest
        {
            Model = "qwen2.5:1.5b",
            Prompt = prompt,
            Stream = false
        };

        var response = await httpClient.PostAsJsonAsync(
            "api/generate",
            request,
            cancellationToken);

        response.EnsureSuccessStatusCode();

        var result = await response.Content
            .ReadFromJsonAsync<OllamaResponse>(
                cancellationToken);

        return result?.Response ?? string.Empty;
    }

    public async Task<string> GenerateJsonAsync(
        string prompt,
        CancellationToken cancellationToken = default)
    {
        var request = new OllamaRequest
        {
            Model = "qwen2.5:1.5b",
            Prompt = prompt,
            Stream = false,
            Format = "json"
        };

        var response = await httpClient.PostAsJsonAsync(
            "api/generate",
            request,
            cancellationToken);

        response.EnsureSuccessStatusCode();

        var result = await response.Content
            .ReadFromJsonAsync<OllamaResponse>(
                cancellationToken);

        return result?.Response ?? string.Empty;
    }

    private class OllamaRequest
    {
        public string Model { get; set; } = string.Empty;

        public string Prompt { get; set; } = string.Empty;

        public bool Stream { get; set; }

        public string? Format { get; set; }
    }

    private class OllamaResponse
    {
        [JsonPropertyName("response")]
        public string Response { get; set; } = string.Empty;
    }
}
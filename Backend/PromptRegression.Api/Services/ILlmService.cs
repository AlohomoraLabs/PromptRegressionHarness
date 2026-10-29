namespace PromptRegression.Api.Services
{
    public interface ILlmService
    {
        Task<string> GenerateAsync(
            string prompt,
            CancellationToken cancellationToken = default);

        Task<string> GenerateJsonAsync(
            string prompt,
            CancellationToken cancellationToken = default);
    }
}

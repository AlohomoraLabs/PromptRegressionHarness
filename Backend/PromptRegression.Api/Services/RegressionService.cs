using PromptRegression.Api.Data;
using PromptRegression.Api.Models;

namespace PromptRegression.Api.Services
{
    public class RegressionService(ILlmService llmService, EmailEvaluator emailEvaluator, HallucinationEvaluator hallucinationEvaluator
        , ToneEvaluator toneEvaluator)
    {
        public async Task<EvaluationResult> RunAsync(
       RegressionRequest request,
       CancellationToken cancellationToken = default)
        {
            var prompt = BuildPrompt(
                request.Prompt,
                request.Homeowner);

            var email = await llmService.GenerateAsync(
                prompt,
                cancellationToken);

            var result = emailEvaluator.Evaluate(
                request.Homeowner,
                email);

            var hallucinationResult =
                await hallucinationEvaluator.EvaluateAsync(
                    request.Homeowner,
                    email,
                    cancellationToken);

            result.Checks.Add(new CheckResult
            {
                Name = "Hallucination",
                Passed = hallucinationResult.Passed,
                Message = hallucinationResult.Passed
                    ? "No unsupported factual claims found."
                    : "Unsupported factual claims were detected."
            });

            var toneResult = await toneEvaluator.EvaluateAsync(
    email,
    cancellationToken);

            result.Checks.Add(new CheckResult
            {
                Name = "Tone & spam",
                Passed = toneResult.Passed,
                Message = toneResult.Passed
                    ? $"Tone: {toneResult.ToneScore}/10, Spam: {toneResult.SpamScore}/10. {toneResult.Reason}"
                    : $"Tone: {toneResult.ToneScore}/10, Spam: {toneResult.SpamScore}/10. {toneResult.Reason}"
            });

            result.Passed = result.Checks.All(x => x.Passed);

            return result;
        }

        private static string BuildPrompt(string prompt, Homeowner homeowner)
        {
            return $"""
                {prompt}

                Homeowner information:

                Name: {homeowner.Name}
                Address: {homeowner.Address}
                Years Owned: {homeowner.YearsOwned?.ToString() ?? "Not provided"}
                Estimated Equity: {homeowner.EstimatedEquity?.ToString("C") ?? "Not provided"}

                Important:
                Only use facts provided above.
                Do not invent or assume any additional information.
                """;
        }

        public async Task<BatchRegressionResult> RunBatchAsync(string prompt, CancellationToken cancellationToken = default)
        {
            var homeowners = HomeownerData.GetAll();

            var results = new List<EvaluationResult>();

            foreach (var homeowner in homeowners)
            {
                var request = new RegressionRequest
                {
                    Homeowner = homeowner,
                    Prompt = prompt
                };

                var result = await RunAsync(
                    request,
                    cancellationToken);

                results.Add(result);
            }

            var passed = results.Count(x => x.Passed);
            var failed = results.Count - passed;

            return new BatchRegressionResult
            {
                TotalTests = results.Count,
                PassedTests = passed,
                FailedTests = failed,
                PassRate = results.Count == 0
                    ? 0
                    : Math.Round(
                        (double)passed / results.Count * 100,
                        2),
                Results = results
            };
        }


        public async Task<PromptComparisonResult> CompareAsync(
        string promptV1,
        string promptV2,
        CancellationToken cancellationToken = default)
        {
            var v1 = await RunBatchAsync(
                promptV1,
                cancellationToken);

            var v2 = await RunBatchAsync(
                promptV2,
                cancellationToken);

            return new PromptComparisonResult
            {
                V1 = v1,
                V2 = v2,
                Improvement = Math.Round(
                    v2.PassRate - v1.PassRate,
                    2)
            };
        }
    }
}
using PromptRegression.Api.Models;
using System.Text.Json;

namespace PromptRegression.Api.Services
{
    public class HallucinationEvaluator(
    ILlmService llmService)
    {
        public async Task<HallucinationResult> EvaluateAsync(
            Homeowner homeowner,
            string email,
            CancellationToken cancellationToken = default)
        {
            var prompt = $$"""
            You are an AI output evaluator.

            Your job is to determine whether an email contains
            factual claims that are not supported by the source data.

            SOURCE DATA:

            Name: {{homeowner.Name}}
            Address: {{homeowner.Address}}
            Years Owned: {{homeowner.YearsOwned?.ToString() ?? "Not provided"}}
            Estimated Equity: {{homeowner.EstimatedEquity?.ToString() ?? "Not provided"}}

            GENERATED EMAIL:

            {{email}}

            RULES:

            1. Every factual claim about the homeowner or property
               must be supported by the source data.
            2. If a value is "Not provided", the email must not invent it.
            3. Do not treat normal greetings or opinions as factual claims.
            4. Identify each important factual claim.
            5. Mark each claim as supported or unsupported.

            Return a JSON object with this exact structure:

            {
              "passed": true,
              "claims": [
                {
                  "claim": "The homeowner has owned the property for 8 years.",
                  "supported": true,
                  "reason": "The source data says Years Owned is 8."
                }
              ]
            }

            Rules for "passed":

            - true only if EVERY factual claim is supported.
            - false if even ONE factual claim is unsupported.

            If there are no factual claims, return:

            {
              "passed": true,
              "claims": []
            }

            Return JSON only.
            Do not include markdown.
            Do not include ```json.
            Do not include explanations outside the JSON object.
            """;

            var response = await llmService.GenerateJsonAsync(
                prompt,
                cancellationToken);

            return ParseResult(response);
        }

        private static HallucinationResult ParseResult(
            string response)
        {
            try
            {
                return JsonSerializer.Deserialize<HallucinationResult>(
                    response,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    })
                    ?? new HallucinationResult
                    {
                        Passed = false
                    };
            }
            catch
            {
                return new HallucinationResult
                {
                    Passed = false,
                    Claims =
                    [
                        new ClaimResult
                    {
                        Claim = "Evaluator response",
                        Supported = false,
                        Reason = "Could not parse evaluator response as JSON."
                    }
                    ]
                };
            }
        }
    }
}

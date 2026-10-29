namespace PromptRegression.Api.Models
{
    public class HallucinationResult
    {
        public bool Passed { get; set; }

        public List<ClaimResult> Claims { get; set; } = [];
    }
}

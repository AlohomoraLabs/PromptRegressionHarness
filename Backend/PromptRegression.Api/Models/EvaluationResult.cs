namespace PromptRegression.Api.Models
{
    public class EvaluationResult
    {
        public bool Passed { get; set; }

        public string GeneratedEmail { get; set; } = string.Empty;

        public List<CheckResult> Checks { get; set; } = [];
    }

}

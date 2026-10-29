namespace PromptRegression.Api.Models
{
    public class ToneEvaluationResult
    {
        public bool Passed { get; set; }

        public int ToneScore { get; set; }

        public int SpamScore { get; set; }

        public string Reason { get; set; } = string.Empty;
    }
}

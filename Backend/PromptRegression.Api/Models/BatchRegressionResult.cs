namespace PromptRegression.Api.Models
{
    public class BatchRegressionResult
    {
        public int TotalTests { get; set; }

        public int PassedTests { get; set; }

        public int FailedTests { get; set; }

        public double PassRate { get; set; }

        public List<EvaluationResult> Results { get; set; } = [];
    }
}

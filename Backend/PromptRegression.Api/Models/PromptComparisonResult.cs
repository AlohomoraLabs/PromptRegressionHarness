namespace PromptRegression.Api.Models
{
    public class PromptComparisonResult
    {
        public BatchRegressionResult V1 { get; set; } = new();

        public BatchRegressionResult V2 { get; set; } = new();

        public double Improvement { get; set; }
    }
}

namespace PromptRegression.Api.Models
{
    public class CheckResult
    {
        public string Name { get; set; } = string.Empty;

        public bool Passed { get; set; }

        public string Message { get; set; } = string.Empty;
    } 
}

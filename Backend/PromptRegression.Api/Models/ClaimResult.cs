namespace PromptRegression.Api.Models
{
    public class ClaimResult
    {
        public string Claim { get; set; } = string.Empty;

        public bool Supported { get; set; }

        public string Reason { get; set; } = string.Empty;
    }
}

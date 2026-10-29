namespace PromptRegression.Api.Models
{
   public class RegressionRequest
   {
        public Homeowner Homeowner { get; set; } = new();

        public string Prompt { get; set; } = string.Empty;
   }
}

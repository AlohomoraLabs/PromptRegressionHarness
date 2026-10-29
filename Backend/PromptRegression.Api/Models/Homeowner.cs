namespace PromptRegression.Api.Models
{
    public class Homeowner
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;

        public int? YearsOwned { get; set; }

        public decimal? EstimatedEquity { get; set; }
    }
}

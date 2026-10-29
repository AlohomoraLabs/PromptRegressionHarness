using PromptRegression.Api.Models;

namespace PromptRegression.Api.Data;

public static class HomeownerData
{
    public static List<Homeowner> GetAll()
    {
        return
        [
            new()
            {
                Id = 1,
                Name = "Sarah Kim",
                Address = "123 Oak Street",
                YearsOwned = 8,
                EstimatedEquity = 120000
            },
            new()
            {
                Id = 2,
                Name = "John Smith",
                Address = "45 Pine Avenue",
                YearsOwned = 2,
                EstimatedEquity = 75000
            },
            new()
            {
                Id = 3,
                Name = "Michael O'Connor",
                Address = "78 Maple Road",
                YearsOwned = null,
                EstimatedEquity = 95000
            }
        ];
    }
}
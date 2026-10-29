using PromptRegression.Api.Models;

namespace PromptRegression.Api.Services
{
    public class EmailEvaluator
    {
        public EvaluationResult Evaluate(
            Homeowner homeowner,
            string email)
        {
            var checks = new List<CheckResult>
        {
            CheckHomeownerName(homeowner, email),
            CheckAddress(homeowner, email),
            CheckYearsOwned(homeowner, email),
            CheckEquity(homeowner, email),
            CheckUnsubscribe(email),
            CheckBannedPhrases(email),
            CheckCallToAction(email),
            CheckEmailLength(email)
        };

            return new EvaluationResult
            {
                Passed = checks.All(x => x.Passed),
                GeneratedEmail = email,
                Checks = checks
            };
        }

        private CheckResult CheckHomeownerName(
       Homeowner homeowner,
       string email)
        {
            var nameParts = homeowner.Name
                .Split(
                    ' ',
                    StringSplitOptions.RemoveEmptyEntries);

            var passed = nameParts.Any(part =>
                part.Length >= 3 &&
                email.Contains(
                    part,
                    StringComparison.OrdinalIgnoreCase));

            return new CheckResult
            {
                Name = "Homeowner name",
                Passed = passed,
                Message = passed
                    ? "Homeowner name is referenced."
                    : "Homeowner name is missing."
            };
        }

        private CheckResult CheckAddress(
            Homeowner homeowner,
            string email)
        {
            var passed = email.Contains(
                homeowner.Address,
                StringComparison.OrdinalIgnoreCase);

            return new CheckResult
            {
                Name = "Physical address",
                Passed = passed,
                Message = passed
                    ? "Correct address used."
                    : "Address is missing or incorrect."
            };
        }

        private CheckResult CheckYearsOwned(
            Homeowner homeowner,
            string email)
        {
            if (homeowner.YearsOwned is null)
            {
                return new CheckResult
                {
                    Name = "Years owned",
                    Passed = true,
                    Message = "No years-owned data was provided."
                };
            }

            var expected = homeowner.YearsOwned.Value.ToString();

            var passed = email.Contains(
                expected,
                StringComparison.OrdinalIgnoreCase);

            return new CheckResult
            {
                Name = "Years owned",
                Passed = passed,
                Message = passed
                    ? "Correct ownership duration used."
                    : "Ownership duration is missing or incorrect."
            };
        }

        private CheckResult CheckEquity(
    Homeowner homeowner,
    string email)
        {
            if (homeowner.EstimatedEquity is null)
            {
                return new CheckResult
                {
                    Name = "Equity",
                    Passed = !ContainsCurrencyAmount(email),
                    Message = ContainsCurrencyAmount(email)
                        ? "Email contains an equity-related value even though none was provided."
                        : "No unsupported equity value found."
                };
            }

            var expected = homeowner.EstimatedEquity.Value
                .ToString("0");

            var normalizedEmail = new string(
                email.Where(char.IsDigit).ToArray());

            var normalizedExpected = new string(
                expected.Where(char.IsDigit).ToArray());

            var passed = normalizedEmail.Contains(
                normalizedExpected,
                StringComparison.Ordinal);

            return new CheckResult
            {
                Name = "Equity",
                Passed = passed,
                Message = passed
                    ? "Correct equity value used."
                    : "Equity value is missing or incorrect."
            };
        }

        private CheckResult CheckUnsubscribe(string email)
        {
            var passed = email.Contains(
                "unsubscribe",
                StringComparison.OrdinalIgnoreCase);

            return new CheckResult
            {
                Name = "Unsubscribe",
                Passed = passed,
                Message = passed
                    ? "Unsubscribe line is present."
                    : "Unsubscribe line is missing."
            };
        }

        private CheckResult CheckBannedPhrases(string email)
        {
            var bannedPhrases = new[]
            {
            "act now",
            "guaranteed"
        };

            var foundPhrase = bannedPhrases.FirstOrDefault(
                phrase => email.Contains(
                    phrase,
                    StringComparison.OrdinalIgnoreCase));

            var passed = foundPhrase is null;

            return new CheckResult
            {
                Name = "Banned phrases",
                Passed = passed,
                Message = passed
                    ? "No banned phrases found."
                    : $"Banned phrase found: '{foundPhrase}'."
            };
        }

        private CheckResult CheckCallToAction(string email)
        {
            var ctaPhrases = new[]
            {
        "reply",
        "contact",
        "call",
        "text",
        "reach out",
        "let me know",
        "get in touch"
    };

            var passed = ctaPhrases.Any(phrase =>
                email.Contains(
                    phrase,
                    StringComparison.OrdinalIgnoreCase));

            return new CheckResult
            {
                Name = "Clear CTA",
                Passed = passed,
                Message = passed
                    ? "A clear call to action is present."
                    : "No clear call to action was found."
            };
        }

        private static string NormalizeNumber(string value)
        {
            return new string(
                value.Where(char.IsDigit).ToArray());
        }

        private static List<string> ExtractNumbers(string text)
        {
            return text
                .Split(
                    ' ',
                    StringSplitOptions.RemoveEmptyEntries)
                .Select(NormalizeNumber)
                .Where(x => !string.IsNullOrEmpty(x))
                .ToList();
        }

        private static bool ContainsCurrencyAmount(string text)
        {
            var currencySymbols = new[]
            {
        "$",
        "€",
        "£",
        "Ksh",
        "KES"
    };

            return currencySymbols.Any(symbol =>
                text.Contains(
                    symbol,
                    StringComparison.OrdinalIgnoreCase));
        }

        private CheckResult CheckEmailLength(string email)
        {
            const int maxLength = 500;

            var passed = email.Length <= maxLength;

            return new CheckResult
            {
                Name = "Email length",
                Passed = passed,
                Message = passed
                    ? $"Email is within the {maxLength}-character limit."
                    : $"Email is {email.Length} characters long, exceeding the {maxLength}-character limit."
            };
        }
    }
}
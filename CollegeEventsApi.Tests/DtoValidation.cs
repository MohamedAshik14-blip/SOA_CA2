using System.ComponentModel.DataAnnotations;

namespace CollegeEventsApi.Tests.TestHelpers;

public static class DtoValidation
{
    public static List<ValidationResult> Validate(object dto)
    {
        var results = new List<ValidationResult>();
        var ctx = new ValidationContext(dto, serviceProvider: null, items: null);

        Validator.TryValidateObject(dto, ctx, results, validateAllProperties: true);

        return results;
    }

    public static bool HasErrorFor(List<ValidationResult> results, string memberName)
        => results.Any(r => r.MemberNames.Any(m => string.Equals(m, memberName, StringComparison.OrdinalIgnoreCase)));

    public static string ErrorsToText(List<ValidationResult> results)
        => string.Join(" | ", results.Select(r => $"{string.Join(",", r.MemberNames)}: {r.ErrorMessage}"));
}
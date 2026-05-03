namespace TechsysLog.Application.Common.Errors;

public record ValidationFieldError(string Field, string Message);

public record ValidationProblem(string Type, IReadOnlyList<ValidationFieldError> Errors)
{
    public static ValidationProblem FromFluent(IEnumerable<FluentValidation.Results.ValidationFailure> failures)
        => new("validation_error",
            failures.Select(f => new ValidationFieldError(ToCamelCase(f.PropertyName), f.ErrorMessage)).ToList());

    private static string ToCamelCase(string name)
    {
        if (string.IsNullOrEmpty(name)) return name;
        var lastDot = name.LastIndexOf('.');
        var prop = lastDot >= 0 ? name[(lastDot + 1)..] : name;
        return char.ToLowerInvariant(prop[0]) + prop[1..];
    }
}

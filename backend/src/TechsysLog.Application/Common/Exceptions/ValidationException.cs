using FluentValidation.Results;

namespace TechsysLog.Application.Common.Exceptions;

public class ValidationException : Exception
{
    public IReadOnlyList<ValidationFailure> Failures { get; }

    public ValidationException(IEnumerable<ValidationFailure> failures)
        : base("One or more validation failures occurred.")
    {
        Failures = failures.ToList();
    }
}

using FluentValidation;

namespace TechsysLog.Application.Features.Cep.LookupCep;

public class LookupCepValidator : AbstractValidator<LookupCepQuery>
{
    public LookupCepValidator()
    {
        RuleFor(x => x.Cep)
            .NotEmpty()
            .Matches(@"^\d{8}$")
            .WithMessage("CEP deve conter exatamente 8 dígitos.");
    }
}

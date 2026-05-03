using FluentValidation;

namespace TechsysLog.Application.Features.Dashboard.GetDashboard;

public class GetDashboardValidator : AbstractValidator<GetDashboardQuery>
{
    public GetDashboardValidator()
    {
        RuleFor(x => x.Month)
            .InclusiveBetween(1, 12)
            .WithMessage("Mês deve estar entre 1 e 12.");

        RuleFor(x => x.Year)
            .InclusiveBetween(2000, 2100)
            .WithMessage("Ano deve estar entre 2000 e 2100.");
    }
}

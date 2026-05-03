using FluentValidation;

namespace TechsysLog.Application.Features.Orders.GetOrderDetails;

public class GetOrderDetailsValidator : AbstractValidator<GetOrderDetailsQuery>
{
    public GetOrderDetailsValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Id é obrigatório.");
    }
}

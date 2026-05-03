using FluentValidation;

namespace TechsysLog.Application.Features.Orders.CreateOrder;

public class CreateOrderValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderValidator()
    {
        RuleFor(x => x.OrderNumber)
            .NotEmpty().WithMessage("Número do pedido é obrigatório.")
            .Matches(@"^\d+$").WithMessage("Número do pedido deve conter apenas dígitos.")
            .MaximumLength(20).WithMessage("Número do pedido deve ter no máximo 20 dígitos.");

        RuleFor(x => x.Description).NotEmpty();
        RuleFor(x => x.Value).GreaterThan(0);
        RuleFor(x => x.Cep).NotEmpty().Matches(@"^\d{8}$").WithMessage("CEP must contain 8 digits.");
        RuleFor(x => x.Number).NotEmpty();

        RuleFor(x => x.Complement)
            .MaximumLength(100)
            .When(x => x.Complement is not null)
            .WithMessage("Complemento deve ter no máximo 100 caracteres.");

        RuleFor(x => x.Observation)
            .MaximumLength(500)
            .When(x => x.Observation is not null)
            .WithMessage("Observação deve ter no máximo 500 caracteres.");
    }
}

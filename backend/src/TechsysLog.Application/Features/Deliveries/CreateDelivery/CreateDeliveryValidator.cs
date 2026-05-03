using FluentValidation;

namespace TechsysLog.Application.Features.Deliveries.CreateDelivery;

public class CreateDeliveryValidator : AbstractValidator<CreateDeliveryCommand>
{
    public CreateDeliveryValidator()
    {
        RuleFor(x => x.OrderId).NotEmpty();
        RuleFor(x => x.DeliveredAt).NotEmpty();
    }
}

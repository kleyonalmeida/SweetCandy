using Application.Features.Orders.Commands;
using FluentValidation;

namespace Application.Features.Orders.Validations;

public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
  public CreateOrderCommandValidator()
  {
    RuleFor(command => command.CreateOrder)
      .NotNull();

    RuleFor(command => command.CreateOrder.Name)
      .NotEmpty().WithMessage("Nome do cliente é obrigatório.");

    RuleForEach(command => command.CreateOrder.Items)
      .ChildRules(item =>
      {
        item.RuleFor(orderItem => orderItem.Quantity)
          .GreaterThan(0);

        item.RuleFor(orderItem => orderItem.FinalProductName)
          .NotEmpty()
          .When(orderItem => string.IsNullOrWhiteSpace(orderItem.FinalProductId));
      });
  }
}
using Application.Features.Orders.Commands;
using FluentValidation;

namespace Application.Features.Orders.Validations;

public class UpdateOrderCommandValidator : AbstractValidator<UpdateOrderCommand>
{
  public UpdateOrderCommandValidator()
  {
    RuleFor(command => command.Id)
      .NotEmpty();

    RuleFor(command => command.UpdateOrder)
      .NotNull();

    RuleFor(command => command.UpdateOrder.Name)
      .NotEmpty().When(x => x.UpdateOrder.Name != null)
      .WithMessage("Nome do cliente é obrigatório quando presente.");

    RuleForEach(command => command.UpdateOrder.Items)
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
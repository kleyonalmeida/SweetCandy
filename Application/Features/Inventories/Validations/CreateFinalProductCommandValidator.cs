using Application.Features.Inventories.Commands;
using FluentValidation;

namespace Application.Features.Inventories.Validations;

public class CreateFinalProductCommandValidator : AbstractValidator<CreateFinalProductCommand>
{
  public CreateFinalProductCommandValidator()
  {
    RuleFor(command => command.FinalProduct)
      .NotNull();

    RuleFor(command => command.FinalProduct.Name)
      .NotEmpty();

    RuleFor(command => command.FinalProduct.CostPrice)
      .GreaterThanOrEqualTo(0)
      .When(command => command.FinalProduct.CostPrice.HasValue);

    RuleFor(command => command.FinalProduct.UnitPrice)
      .GreaterThanOrEqualTo(0)
      .When(command => command.FinalProduct.UnitPrice.HasValue);

    RuleFor(command => command.FinalProduct.QuantityAvailable)
      .GreaterThanOrEqualTo(0)
      .When(command => command.FinalProduct.QuantityAvailable.HasValue);
  }
}
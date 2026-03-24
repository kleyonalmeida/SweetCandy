using Application.Features.Inventories.Commands;
using FluentValidation;

namespace Application.Features.Inventories.Validations;

public class CreateSupplyCommandValidator : AbstractValidator<CreateSupplyCommand>
{
  public CreateSupplyCommandValidator()
  {
    RuleFor(command => command.Supply)
      .NotNull();

    RuleFor(command => command.Supply.Name)
      .NotEmpty();

    RuleFor(command => command.Supply.Quantity)
      .NotNull()
      .GreaterThan(0);

    RuleFor(command => command.Supply.Price)
      .NotNull()
      .GreaterThanOrEqualTo(0);
  }
}
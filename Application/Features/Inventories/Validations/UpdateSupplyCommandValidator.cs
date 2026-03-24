using Application.Features.Inventories.Commands;
using FluentValidation;

namespace Application.Features.Inventories.Validations;

public class UpdateSupplyCommandValidator : AbstractValidator<UpdateSupplyCommand>
{
  public UpdateSupplyCommandValidator()
  {
    RuleFor(command => command.Id)
      .NotEmpty();

    RuleFor(command => command.Supply.Quantity)
      .GreaterThan(0)
      .When(command => command.Supply.Quantity.HasValue);

    RuleFor(command => command.Supply.Price)
      .GreaterThanOrEqualTo(0)
      .When(command => command.Supply.Price.HasValue);
  }
}
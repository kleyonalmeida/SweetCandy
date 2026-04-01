using Application.Features.StockMovements.Commands;
using FluentValidation;

namespace Application.Features.StockMovements.Validations;

public class CreateStockMovementCommandValidator : AbstractValidator<CreateStockMovementCommand>
{
  public CreateStockMovementCommandValidator()
  {
    RuleFor(command => command.CreateStockMovement)
      .NotNull();

    RuleFor(command => command.CreateStockMovement.SupplyId)
      .NotEmpty();

    RuleFor(command => command.CreateStockMovement.Quantity)
      .GreaterThan(0);
  }
}

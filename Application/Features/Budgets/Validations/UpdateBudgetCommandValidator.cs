using Application.Features.Budget.Commands;
using FluentValidation;

namespace Application.Features.Budget.Validations;

public class UpdateBudgetCommandValidator : AbstractValidator<UpdateBudgetCommand>
{
  public UpdateBudgetCommandValidator()
  {
    RuleFor(command => command.Id)
      .NotEmpty();

    RuleFor(command => command.UpdateBudget)
      .NotNull();

    RuleFor(command => command.UpdateBudget.FinalProductQuantity)
      .GreaterThan(0).When(c => c.UpdateBudget.FinalProductQuantity.HasValue);
  }
}

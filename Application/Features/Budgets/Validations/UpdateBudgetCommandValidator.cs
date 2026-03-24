using Application.Features.Budgets.Commands;
using FluentValidation;

namespace Application.Features.Budgets.Validations;

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

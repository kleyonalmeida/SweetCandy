using Application.Features.Budgets.Commands;
using FluentValidation;

namespace Application.Features.Budgets.Validations;

public class ConvertBudgetToOrderCommandValidator : AbstractValidator<ConvertBudgetToOrderCommand>
{
  public ConvertBudgetToOrderCommandValidator()
  {
    RuleFor(command => command.BudgetId)
      .NotEmpty();
  }
}

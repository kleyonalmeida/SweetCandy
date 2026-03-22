using Application.Features.Budgets.Commands;
using FluentValidation;

namespace Application.Features.Budgets.Validations;

public class CreateBudgetCommandValidator : AbstractValidator<CreateBudgetCommand>
{
  public CreateBudgetCommandValidator()
  {
    RuleFor(command => command.CreateBudget)
      .NotNull();

    RuleFor(command => command.CreateBudget.ClientName)
      .NotEmpty();

    RuleFor(command => command.CreateBudget.FinalProductQuantity)
      .GreaterThan(0).When(c => c.CreateBudget.FinalProductQuantity.HasValue);
  }
}

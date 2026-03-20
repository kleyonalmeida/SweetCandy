using Application.Features.Budget.Commands;
using FluentValidation;

namespace Application.Features.Budget.Validations;

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

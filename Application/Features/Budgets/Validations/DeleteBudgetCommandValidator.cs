using Application.Features.Budgets.Commands;
using FluentValidation;

namespace Application.Features.Budgets.Validations;

public class DeleteBudgetCommandValidator : AbstractValidator<DeleteBudgetCommand>
{
  public DeleteBudgetCommandValidator()
  {
    RuleFor(command => command.Id)
      .NotEmpty();
  }
}
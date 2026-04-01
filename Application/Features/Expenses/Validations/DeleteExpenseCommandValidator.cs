using Application.Features.Expenses.Commands;
using FluentValidation;

namespace Application.Features.Expenses.Validations;

public class DeleteExpenseCommandValidator : AbstractValidator<DeleteExpenseCommand>
{
  public DeleteExpenseCommandValidator()
  {
    RuleFor(command => command.Id)
      .NotEmpty();
  }
}

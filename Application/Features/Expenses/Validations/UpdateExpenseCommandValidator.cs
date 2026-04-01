using Application.Features.Expenses.Commands;
using FluentValidation;

namespace Application.Features.Expenses.Validations;

public class UpdateExpenseCommandValidator : AbstractValidator<UpdateExpenseCommand>
{
  public UpdateExpenseCommandValidator()
  {
    RuleFor(command => command.Id)
      .NotEmpty();

    RuleFor(command => command.UpdateExpense)
      .NotNull();

    RuleFor(command => command.UpdateExpense.Value)
      .GreaterThanOrEqualTo(0)
      .When(command => command.UpdateExpense.Value.HasValue);
  }
}

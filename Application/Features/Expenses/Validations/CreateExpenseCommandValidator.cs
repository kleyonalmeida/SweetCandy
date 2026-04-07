using Application.Features.Expenses.Commands;
using Domain.Enums;
using FluentValidation;

namespace Application.Features.Expenses.Validations;

public class CreateExpenseCommandValidator : AbstractValidator<CreateExpenseCommand>
{
  public CreateExpenseCommandValidator()
  {
    RuleFor(command => command.CreateExpense)
      .NotNull();

    RuleFor(command => command.CreateExpense.Name)
      .NotEmpty();

    RuleFor(command => command.CreateExpense.Value)
      .NotNull()
      .GreaterThanOrEqualTo(0);

    RuleFor(command => command.CreateExpense.PaymentMethod)
      .IsInEnum();
  }
}

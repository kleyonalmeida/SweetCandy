using Application.Features.Expenses.Commands;
using FluentValidation;

namespace Application.Features.Expenses.Validations;

public class CreateExpenseCommandValidator : AbstractValidator<CreateExpenseCommand>
{
  public CreateExpenseCommandValidator()
  {
    RuleFor(x => x.Expense).NotNull();
    RuleFor(x => x.Expense!.Name).NotEmpty().WithMessage("Nome da despesa é obrigatório.");
    RuleFor(x => x.Expense!.Value).NotNull().WithMessage("Valor é obrigatório.")
      .GreaterThanOrEqualTo(0).When(x => x.Expense!.Value.HasValue);
  }
}

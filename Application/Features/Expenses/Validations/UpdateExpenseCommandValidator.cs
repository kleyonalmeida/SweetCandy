using Application.Features.Expenses.Commands;
using FluentValidation;

namespace Application.Features.Expenses.Validations;

public class UpdateExpenseCommandValidator : AbstractValidator<UpdateExpenseCommand>
{
  public UpdateExpenseCommandValidator()
  {
    RuleFor(x => x.Id).NotEmpty();
    RuleFor(x => x.UpdateExpense).NotNull();
    RuleFor(x => x.UpdateExpense!.Name).NotEmpty().WithMessage("Nome da despesa é obrigatório.");
    RuleFor(x => x.UpdateExpense!.Value).NotNull().WithMessage("Valor é obrigatório.")
      .GreaterThanOrEqualTo(0).When(x => x.UpdateExpense!.Value.HasValue);
  }
}

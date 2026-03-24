using Application.Features.Receipts.Commands;
using FluentValidation;

namespace Application.Features.Receipts.Validations;

public class UpdateReceiptCommandValidator : AbstractValidator<UpdateReceiptCommand>
{
  public UpdateReceiptCommandValidator()
  {
    RuleFor(command => command.Id)
      .NotEmpty();

    RuleFor(command => command.UpdateReceipt)
      .NotNull();

    RuleFor(command => command.UpdateReceipt.Amount)
      .GreaterThan(0)
      .When(command => command.UpdateReceipt.Amount.HasValue);
  }
}
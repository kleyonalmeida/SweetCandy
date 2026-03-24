using Application.Features.Receipts.Commands;
using FluentValidation;

namespace Application.Features.Receipts.Validations;

public class CreateReceiptCommandValidator : AbstractValidator<CreateReceiptCommand>
{
  public CreateReceiptCommandValidator()
  {
    RuleFor(command => command.CreateReceipt)
      .NotNull();

    RuleFor(command => command.CreateReceipt.Date)
      .NotEmpty();

    RuleFor(command => command.CreateReceipt.Amount)
      .GreaterThan(0);
  }
}
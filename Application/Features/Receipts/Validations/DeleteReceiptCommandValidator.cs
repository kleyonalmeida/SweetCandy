using Application.Features.Receipts.Commands;
using FluentValidation;

namespace Application.Features.Receipts.Validations;

public class DeleteReceiptCommandValidator : AbstractValidator<DeleteReceiptCommand>
{
  public DeleteReceiptCommandValidator()
  {
    RuleFor(command => command.Id)
      .NotEmpty();
  }
}
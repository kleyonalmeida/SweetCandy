using Application.Features.Inventories.Commands;
using FluentValidation;

namespace Application.Features.Inventories.Validations;

public class DeleteSupplyCommandValidator : AbstractValidator<DeleteSupplyCommand>
{
  public DeleteSupplyCommandValidator()
  {
    RuleFor(command => command.Id)
      .NotEmpty();
  }
}
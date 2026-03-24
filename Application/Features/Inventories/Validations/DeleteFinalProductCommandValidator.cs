using Application.Features.Inventories.Commands;
using FluentValidation;

namespace Application.Features.Inventories.Validations;

public class DeleteFinalProductCommandValidator : AbstractValidator<DeleteFinalProductCommand>
{
  public DeleteFinalProductCommandValidator()
  {
    RuleFor(command => command.Id)
      .NotEmpty();
  }
}
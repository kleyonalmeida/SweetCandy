using Application.Features.Orders.Commands;
using FluentValidation;

namespace Application.Features.Orders.Validations;

public class DeleteOrderCommandValidator : AbstractValidator<DeleteOrderCommand>
{
  public DeleteOrderCommandValidator()
  {
    RuleFor(command => command.Id)
      .NotEmpty();
  }
}
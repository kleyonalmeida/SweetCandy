using Application.Features.Customers.Commands;
using FluentValidation;

namespace Application.Features.Customers.Validations;

public class DeleteCustomerCommandValidator : AbstractValidator<DeleteCustomerCommand>
{
  public DeleteCustomerCommandValidator()
  {
    RuleFor(command => command.Id)
      .NotEmpty();
  }
}

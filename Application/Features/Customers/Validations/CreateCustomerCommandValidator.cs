using Application.Features.Customers.Commands;
using FluentValidation;

namespace Application.Features.Customers.Validations;

public class CreateCustomerCommandValidator : AbstractValidator<CreateCustomerCommand>
{
  public CreateCustomerCommandValidator()
  {
    RuleFor(command => command.CreateCustomer)
      .NotNull();

    RuleFor(command => command.CreateCustomer.Name)
      .NotEmpty();
  }
}

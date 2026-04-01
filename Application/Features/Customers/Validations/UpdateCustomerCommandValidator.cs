using Application.Features.Customers.Commands;
using FluentValidation;

namespace Application.Features.Customers.Validations;

public class UpdateCustomerCommandValidator : AbstractValidator<UpdateCustomerCommand>
{
  public UpdateCustomerCommandValidator()
  {
    RuleFor(command => command.Id)
      .NotEmpty();

    RuleFor(command => command.UpdateCustomer)
      .NotNull();

    RuleFor(command => command.UpdateCustomer.Name)
      .NotEmpty()
      .When(x => x.UpdateCustomer.Name != null)
      .WithMessage("Nome do cliente nao pode ser vazio quando informado.");
  }
}

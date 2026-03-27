using Application.Features.Customers.Commands;
using FluentValidation;

namespace Application.Features.Customers.Validations;

public class CreateCustomerCommandValidator : AbstractValidator<CreateCustomerCommand>
{
  public CreateCustomerCommandValidator()
  {
    RuleFor(x => x.Customer).NotNull();
    RuleFor(x => x.Customer!.Name).NotEmpty().WithMessage("Nome do cliente é obrigatório.");
    RuleFor(x => x.Customer!.Email).EmailAddress().When(x => !string.IsNullOrWhiteSpace(x.Customer!.Email));
    RuleFor(x => x.Customer!.Phone).NotEmpty().WithMessage("Telefone do cliente é obrigatório.");
  }
}

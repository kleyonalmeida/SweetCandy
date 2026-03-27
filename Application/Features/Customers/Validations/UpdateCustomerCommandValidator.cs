using Application.Features.Customers.Commands;
using FluentValidation;

namespace Application.Features.Customers.Validations;

public class UpdateCustomerCommandValidator : AbstractValidator<UpdateCustomerCommand>
{
  public UpdateCustomerCommandValidator()
  {
    RuleFor(x => x.Id).NotEmpty().WithMessage("Id do cliente é obrigatório.");
    RuleFor(x => x.Customer).NotNull();
    RuleFor(x => x.Customer!.Name).NotEmpty().WithMessage("Nome do cliente é obrigatório.");
    RuleFor(x => x.Customer!.Email).EmailAddress().When(x => !string.IsNullOrWhiteSpace(x.Customer!.Email));
    RuleFor(x => x.Customer!.Phone).NotEmpty().WithMessage("Telefone do cliente é obrigatório.");
  }
}

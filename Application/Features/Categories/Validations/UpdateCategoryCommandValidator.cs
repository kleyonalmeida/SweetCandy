using Application.Features.Categories.Commands;
using FluentValidation;

namespace Application.Features.Categories.Validations;

public class UpdateCategoryCommandValidator : AbstractValidator<UpdateCategoryCommand>
{
  public UpdateCategoryCommandValidator()
  {
    RuleFor(command => command.Id)
      .NotEmpty();

    RuleFor(command => command.UpdateCategory)
      .NotNull();

    RuleFor(command => command.UpdateCategory.Name)
      .NotEmpty()
      .When(x => x.UpdateCategory.Name != null)
      .WithMessage("Nome da categoria nao pode ser vazio quando informado.");
  }
}

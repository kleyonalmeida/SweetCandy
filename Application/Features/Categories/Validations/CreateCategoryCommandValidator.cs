using Application.Features.Categories.Commands;
using FluentValidation;

namespace Application.Features.Categories.Validations;

public class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>
{
  public CreateCategoryCommandValidator()
  {
    RuleFor(command => command.CreateCategory)
      .NotNull();

    RuleFor(command => command.CreateCategory.Name)
      .NotEmpty();
  }
}

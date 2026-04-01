using Application.Features.Categories.Commands;
using FluentValidation;

namespace Application.Features.Categories.Validations;

public class DeleteCategoryCommandValidator : AbstractValidator<DeleteCategoryCommand>
{
  public DeleteCategoryCommandValidator()
  {
    RuleFor(command => command.Id)
      .NotEmpty();
  }
}

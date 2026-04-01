using Application.Common.Mappings;
using Application.Features.Categories.DTOs;
using Application.Pipelines;
using Application.Wrappers;
using Mapster;
using MediatR;

namespace Application.Features.Categories.Commands;

public record UpdateCategoryCommand(string Id, UpdateCategoryRequest UpdateCategory) : IRequest<IResponseWrapper>, IValidateMe;

public class UpdateCategoryCommandHandler(ICategoryService categoryService) : IRequestHandler<UpdateCategoryCommand, IResponseWrapper>
{
  private readonly ICategoryService _categoryService = categoryService;

  public async Task<IResponseWrapper> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
  {
    var category = await _categoryService.GetByIdAsync(request.Id);

    if (category is null)
      return await ResponseWrapper.FailAsync("Categoria nao encontrada.");

    request.UpdateCategory.Adapt(category, MapsterSettings.IgnoreNullValues);

    category.UpdatedAt = DateTime.UtcNow;

    var serviceMessage = await _categoryService.UpdateAsync(category);
    var successMessage = string.IsNullOrWhiteSpace(serviceMessage)
      ? "Categoria atualizada com sucesso."
      : serviceMessage;

    return await ResponseWrapper.SuccessAsync(successMessage);
  }
}

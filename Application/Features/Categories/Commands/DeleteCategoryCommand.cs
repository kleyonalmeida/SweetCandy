using Application.Pipelines;
using Application.Wrappers;
using MediatR;

namespace Application.Features.Categories.Commands;

public record DeleteCategoryCommand(string Id) : IRequest<IResponseWrapper>, IValidateMe;

public class DeleteCategoryCommandHandler(ICategoryService categoryService) : IRequestHandler<DeleteCategoryCommand, IResponseWrapper>
{
  private readonly ICategoryService _categoryService = categoryService;

  public async Task<IResponseWrapper> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
  {
    var category = await _categoryService.GetByIdAsync(request.Id);

    if (category is null)
      return await ResponseWrapper.FailAsync("Categoria nao encontrada.");

    var serviceMessage = await _categoryService.DeleteAsync(category);
    var successMessage = string.IsNullOrWhiteSpace(serviceMessage)
      ? "Categoria removida com sucesso."
      : serviceMessage;

    return await ResponseWrapper.SuccessAsync(successMessage);
  }
}

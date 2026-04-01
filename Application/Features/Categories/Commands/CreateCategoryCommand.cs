using Application.Features.Categories.DTOs;
using Application.Pipelines;
using Application.Wrappers;
using Domain.Entities;
using Mapster;
using MediatR;

namespace Application.Features.Categories.Commands;

public class CreateCategoryCommand(CreateCategoryRequest createCategory) : IRequest<IResponseWrapper>, IValidateMe
{
  public CreateCategoryRequest CreateCategory { get; set; } = createCategory;
}

public class CreateCategoryCommandHandler(ICategoryService categoryService) : IRequestHandler<CreateCategoryCommand, IResponseWrapper>
{
  private readonly ICategoryService _categoryService = categoryService;

  public async Task<IResponseWrapper> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
  {
    var category = request.CreateCategory.Adapt<Category>();

    var createdCategoryId = await _categoryService.CreateAsync(category);

    return await ResponseWrapper.SuccessAsync($"Categoria criada com sucesso. Id: {createdCategoryId}");
  }
}

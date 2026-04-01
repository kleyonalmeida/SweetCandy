using Application.Features.Categories.DTOs;
using Application.Wrappers;
using Mapster;
using MediatR;

namespace Application.Features.Categories.Queries;

public record GetCategoryByIdQuery(string Id) : IRequest<ResponseWrapper<CategoryResponse>>;

public class GetCategoryByIdQueryHandler(ICategoryService categoryService) : IRequestHandler<GetCategoryByIdQuery, ResponseWrapper<CategoryResponse>>
{
  private readonly ICategoryService _categoryService = categoryService;

  public async Task<ResponseWrapper<CategoryResponse>> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
  {
    var category = await _categoryService.GetByIdAsync(request.Id);

    if (category is null)
      return await ResponseWrapper<CategoryResponse>.FailAsync("Categoria nao encontrada.");

    return await ResponseWrapper<CategoryResponse>.SuccessAsync(category.Adapt<CategoryResponse>());
  }
}

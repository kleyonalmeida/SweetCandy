using Application.Features.Categories.DTOs;
using Application.Wrappers;
using Mapster;
using MediatR;

namespace Application.Features.Categories.Queries;

public record GetCategoriesQuery(int Page = 1, int PageSize = 20) : IRequest<ResponseWrapper<List<CategoryResponse>>>;

public class GetCategoriesQueryHandler(ICategoryService categoryService) : IRequestHandler<GetCategoriesQuery, ResponseWrapper<List<CategoryResponse>>>
{
  private readonly ICategoryService _categoryService = categoryService;

  public async Task<ResponseWrapper<List<CategoryResponse>>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
  {
    var categories = await _categoryService.GetAllAsync();

    var projected = categories
      .Skip((request.Page - 1) * request.PageSize)
      .Take(request.PageSize)
      .Select(category => category.Adapt<CategoryResponse>())
      .ToList();

    return await ResponseWrapper<List<CategoryResponse>>.SuccessAsync(projected);
  }
}

using Application.Features.Categories;
using Application.Features.Inventories.DTOs;
using Application.Pipelines;
using Application.Wrappers;
using Domain.Entities;
using Mapster;
using MediatR;

namespace Application.Features.Inventories.Commands;

public class CreateFinalProductCommand(CreateFinalProductRequest finalProduct) : IRequest<IResponseWrapper>, IValidateMe
{
  public CreateFinalProductRequest FinalProduct { get; set; } = finalProduct;
}

public class CreateFinalProductCommandHandler(
  IInventoryService inventoryService,
  ICategoryService categoryService) : IRequestHandler<CreateFinalProductCommand, IResponseWrapper>
{
  private readonly IInventoryService _inventoryService = inventoryService;
  private readonly ICategoryService _categoryService = categoryService;

  public async Task<IResponseWrapper> Handle(CreateFinalProductCommand request, CancellationToken cancellationToken)
  {
    if (!string.IsNullOrWhiteSpace(request.FinalProduct.CategoryId))
    {
      var category = await _categoryService.GetByIdAsync(request.FinalProduct.CategoryId);
      if (category is null)
        return await ResponseWrapper.FailAsync("Categoria nao encontrada.");
    }

    var finalProduct = request.FinalProduct.Adapt<FinalProduct>();

    var createdFinalProductId = await _inventoryService.CreateFinalProductAsync(finalProduct);

    return await ResponseWrapper.SuccessAsync($"Produto final criado com sucesso. Id: {createdFinalProductId}");
  }
}
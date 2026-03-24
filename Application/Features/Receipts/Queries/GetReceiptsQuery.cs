using Application.Features.Receipts.DTOs;
using Application.Wrappers;
using Mapster;
using MediatR;

namespace Application.Features.Receipts.Queries;

public record GetReceiptsQuery(int Page = 1, int PageSize = 20) : IRequest<ResponseWrapper<List<ReceiptResponse>>>;

public class GetReceiptsQueryHandler(IReceiptsService receiptsService) : IRequestHandler<GetReceiptsQuery, ResponseWrapper<List<ReceiptResponse>>>
{
  private readonly IReceiptsService _receiptsService = receiptsService;

  public async Task<ResponseWrapper<List<ReceiptResponse>>> Handle(GetReceiptsQuery request, CancellationToken cancellationToken)
  {
    var receipts = await _receiptsService.GetAllAsync();

    var projectedReceipts = receipts
      .Skip((request.Page - 1) * request.PageSize)
      .Take(request.PageSize)
      .Select(receipt => receipt.Adapt<ReceiptResponse>())
      .ToList();

    return await ResponseWrapper<List<ReceiptResponse>>.SuccessAsync(projectedReceipts);
  }
}
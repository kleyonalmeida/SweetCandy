using Application.Features.Receipts.DTOs;
using Application.Wrappers;
using Mapster;
using MediatR;

namespace Application.Features.Receipts.Queries;

public record GetReceiptByIdQuery(string Id) : IRequest<ResponseWrapper<ReceiptResponse>>;

public class GetReceiptByIdQueryHandler(IReceiptsService receiptsService) : IRequestHandler<GetReceiptByIdQuery, ResponseWrapper<ReceiptResponse>>
{
  private readonly IReceiptsService _receiptsService = receiptsService;

  public async Task<ResponseWrapper<ReceiptResponse>> Handle(GetReceiptByIdQuery request, CancellationToken cancellationToken)
  {
    var receipt = await _receiptsService.GetByIdAsync(request.Id);

    if (receipt is null)
      return await ResponseWrapper<ReceiptResponse>.FailAsync("Recebimento nao encontrado.");

    return await ResponseWrapper<ReceiptResponse>.SuccessAsync(receipt.Adapt<ReceiptResponse>());
  }
}
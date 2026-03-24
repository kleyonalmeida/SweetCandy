using Application.Common.Mappings;
using Application.Features.Receipts.DTOs;
using Application.Pipelines;
using Application.Wrappers;
using Domain.Entities;
using Mapster;
using MediatR;

namespace Application.Features.Receipts.Commands;

public record UpdateReceiptCommand(string Id, UpdateReceiptRequest UpdateReceipt) : IRequest<IResponseWrapper>, IValidateMe;

public class UpdateReceiptCommandHandler(IReceiptsService receiptsService) : IRequestHandler<UpdateReceiptCommand, IResponseWrapper>
{
  private readonly IReceiptsService _receiptsService = receiptsService;

  public async Task<IResponseWrapper> Handle(UpdateReceiptCommand request, CancellationToken cancellationToken)
  {
    var receipt = await _receiptsService.GetByIdAsync(request.Id);

    if (receipt is null)
      return await ResponseWrapper.FailAsync("Recebimento nao encontrado.");

    request.UpdateReceipt.Adapt(receipt, MapsterSettings.IgnoreNullValues);

    // mapping already handles trimming / null rules globally

    receipt.UpdatedAt = DateTime.UtcNow;

    var serviceMessage = await _receiptsService.UpdateAsync(receipt);
    var successMessage = string.IsNullOrWhiteSpace(serviceMessage)
      ? "Recebimento atualizado com sucesso."
      : serviceMessage;

    return await ResponseWrapper.SuccessAsync(successMessage);
  }

}
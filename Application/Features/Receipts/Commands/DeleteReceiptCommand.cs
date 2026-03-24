using Application.Pipelines;
using Application.Wrappers;
using MediatR;

namespace Application.Features.Receipts.Commands;

public record DeleteReceiptCommand(string Id) : IRequest<IResponseWrapper>, IValidateMe;

public class DeleteReceiptCommandHandler(IReceiptsService receiptsService) : IRequestHandler<DeleteReceiptCommand, IResponseWrapper>
{
  private readonly IReceiptsService _receiptsService = receiptsService;

  public async Task<IResponseWrapper> Handle(DeleteReceiptCommand request, CancellationToken cancellationToken)
  {
    var receipt = await _receiptsService.GetByIdAsync(request.Id);

    if (receipt is null)
      return await ResponseWrapper.FailAsync("Recebimento nao encontrado.");

    var serviceMessage = await _receiptsService.DeleteAsync(receipt);
    var successMessage = string.IsNullOrWhiteSpace(serviceMessage)
      ? "Recebimento removido com sucesso."
      : serviceMessage;

    return await ResponseWrapper.SuccessAsync(successMessage);
  }
}
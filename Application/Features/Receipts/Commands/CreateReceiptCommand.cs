using Application.Features.Receipts.DTOs;
using Application.Pipelines;
using Application.Wrappers;
using Domain.Entities;
using Mapster;
using MediatR;

namespace Application.Features.Receipts.Commands;

public class CreateReceiptCommand(CreateReceiptRequest createReceipt) : IRequest<IResponseWrapper>, IValidateMe
{
  public CreateReceiptRequest CreateReceipt { get; set; } = createReceipt;
}

public class CreateReceiptCommandHandler(IReceiptsService receiptsService) : IRequestHandler<CreateReceiptCommand, IResponseWrapper>
{
  private readonly IReceiptsService _receiptsService = receiptsService;

  public async Task<IResponseWrapper> Handle(CreateReceiptCommand request, CancellationToken cancellationToken)
  {
    var receipt = request.CreateReceipt.Adapt<Receipt>();

    var createdReceiptId = await _receiptsService.CreateAsync(receipt);

    return await ResponseWrapper.SuccessAsync($"Recebimento criado com sucesso. Id: {createdReceiptId}");
  }


}
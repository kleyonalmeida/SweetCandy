using Application.Features.Customers;
using Application.Features.Orders;
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

public class CreateReceiptCommandHandler(
  IReceiptsService receiptsService,
  IOrdersService ordersService,
  ICustomerService customerService) : IRequestHandler<CreateReceiptCommand, IResponseWrapper>
{
  private readonly IReceiptsService _receiptsService = receiptsService;
  private readonly IOrdersService _ordersService = ordersService;
  private readonly ICustomerService _customerService = customerService;

  public async Task<IResponseWrapper> Handle(CreateReceiptCommand request, CancellationToken cancellationToken)
  {
    if (!string.IsNullOrWhiteSpace(request.CreateReceipt.OrderId))
    {
      var order = await _ordersService.GetByIdAsync(request.CreateReceipt.OrderId);
      if (order is null)
        return await ResponseWrapper.FailAsync("Pedido nao encontrado.");
    }

    if (!string.IsNullOrWhiteSpace(request.CreateReceipt.CustomerId))
    {
      var customer = await _customerService.GetByIdAsync(request.CreateReceipt.CustomerId);
      if (customer is null)
        return await ResponseWrapper.FailAsync("Cliente nao encontrado.");
    }

    var receipt = request.CreateReceipt.Adapt<Receipt>();

    var createdReceiptId = await _receiptsService.CreateAsync(receipt);

    return await ResponseWrapper.SuccessAsync($"Recebimento criado com sucesso. Id: {createdReceiptId}");
  }
}
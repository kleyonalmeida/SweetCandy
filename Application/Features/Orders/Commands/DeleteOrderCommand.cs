using Application.Pipelines;
using Application.Wrappers;
using MediatR;

namespace Application.Features.Orders.Commands;

public record DeleteOrderCommand(string Id) : IRequest<IResponseWrapper>, IValidateMe;

public class DeleteOrderCommandHandler(IOrdersService ordersService) : IRequestHandler<DeleteOrderCommand, IResponseWrapper>
{
  private readonly IOrdersService _ordersService = ordersService;

  public async Task<IResponseWrapper> Handle(DeleteOrderCommand request, CancellationToken cancellationToken)
  {
    var order = await _ordersService.GetByIdAsync(request.Id);

    if (order is null)
      return await ResponseWrapper.FailAsync("Pedido nao encontrado.");

    var serviceMessage = await _ordersService.DeleteAsync(order);
    var successMessage = string.IsNullOrWhiteSpace(serviceMessage)
      ? "Pedido removido com sucesso."
      : serviceMessage;

    return await ResponseWrapper.SuccessAsync(successMessage);
  }
}
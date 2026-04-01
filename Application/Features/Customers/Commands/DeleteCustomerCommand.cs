using Application.Pipelines;
using Application.Wrappers;
using MediatR;

namespace Application.Features.Customers.Commands;

public record DeleteCustomerCommand(string Id) : IRequest<IResponseWrapper>, IValidateMe;

public class DeleteCustomerCommandHandler(ICustomerService customerService) : IRequestHandler<DeleteCustomerCommand, IResponseWrapper>
{
  private readonly ICustomerService _customerService = customerService;

  public async Task<IResponseWrapper> Handle(DeleteCustomerCommand request, CancellationToken cancellationToken)
  {
    var customer = await _customerService.GetByIdAsync(request.Id);

    if (customer is null)
      return await ResponseWrapper.FailAsync("Cliente nao encontrado.");

    var serviceMessage = await _customerService.DeleteAsync(customer);
    var successMessage = string.IsNullOrWhiteSpace(serviceMessage)
      ? "Cliente removido com sucesso."
      : serviceMessage;

    return await ResponseWrapper.SuccessAsync(successMessage);
  }
}

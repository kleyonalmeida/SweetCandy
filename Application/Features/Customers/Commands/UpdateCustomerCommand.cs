using Application.Common.Mappings;
using Application.Features.Customers.DTOs;
using Application.Pipelines;
using Application.Wrappers;
using Mapster;
using MediatR;

namespace Application.Features.Customers.Commands;

public record UpdateCustomerCommand(string Id, UpdateCustomerRequest UpdateCustomer) : IRequest<IResponseWrapper>, IValidateMe;

public class UpdateCustomerCommandHandler(ICustomerService customerService) : IRequestHandler<UpdateCustomerCommand, IResponseWrapper>
{
  private readonly ICustomerService _customerService = customerService;

  public async Task<IResponseWrapper> Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
  {
    var customer = await _customerService.GetByIdAsync(request.Id);

    if (customer is null)
      return await ResponseWrapper.FailAsync("Cliente nao encontrado.");

    request.UpdateCustomer.Adapt(customer, MapsterSettings.IgnoreNullValues);

    customer.UpdatedAt = DateTime.UtcNow;

    var serviceMessage = await _customerService.UpdateAsync(customer);
    var successMessage = string.IsNullOrWhiteSpace(serviceMessage)
      ? "Cliente atualizado com sucesso."
      : serviceMessage;

    return await ResponseWrapper.SuccessAsync(successMessage);
  }
}

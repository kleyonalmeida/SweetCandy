using Application.Features.Customers.DTOs;
using Application.Pipelines;
using Application.Wrappers;
using Domain.Entities;
using Mapster;
using MediatR;

namespace Application.Features.Customers.Commands;

public class UpdateCustomerCommand(string id, UpdateCustomerRequest customer) : IRequest<IResponseWrapper>, IValidateMe
{
  public string Id { get; set; } = id;
  public UpdateCustomerRequest Customer { get; set; } = customer;
}

public class UpdateCustomerCommandHandler(ICustomerService customerService) : IRequestHandler<UpdateCustomerCommand, IResponseWrapper>
{
  private readonly ICustomerService _customerService = customerService;

  public async Task<IResponseWrapper> Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
  {
    var customer = request.Customer.Adapt<Customer>();
    customer.Id = request.Id;
    var result = await _customerService.UpdateAsync(customer);

    if (!string.IsNullOrWhiteSpace(result))
      return await ResponseWrapper.FailAsync(result);

    return await ResponseWrapper.SuccessAsync("Cliente atualizado com sucesso.");
  }
}

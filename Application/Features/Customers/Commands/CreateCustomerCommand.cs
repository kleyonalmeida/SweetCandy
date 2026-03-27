using Application.Features.Customers.DTOs;
using Application.Pipelines;
using Application.Wrappers;
using Domain.Entities;
using Mapster;
using MediatR;

namespace Application.Features.Customers.Commands;

public class CreateCustomerCommand(CreateCustomerRequest customer) : IRequest<IResponseWrapper>, IValidateMe
{
  public CreateCustomerRequest Customer { get; set; } = customer;
}

public class CreateCustomerCommandHandler(ICustomerService customerService) : IRequestHandler<CreateCustomerCommand, IResponseWrapper>
{
  private readonly ICustomerService _customerService = customerService;

  public async Task<IResponseWrapper> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
  {
    var customer = request.Customer.Adapt<Customer>();
    var createdId = await _customerService.CreateAsync(customer);
    return await ResponseWrapper.SuccessAsync($"Cliente criado com sucesso. Id: {createdId}");
  }
}

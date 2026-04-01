using Application.Features.Customers.DTOs;
using Application.Wrappers;
using Mapster;
using MediatR;

namespace Application.Features.Customers.Queries;

public record GetCustomerByIdQuery(string Id) : IRequest<ResponseWrapper<CustomerResponse>>;

public class GetCustomerByIdQueryHandler(ICustomerService customerService) : IRequestHandler<GetCustomerByIdQuery, ResponseWrapper<CustomerResponse>>
{
  private readonly ICustomerService _customerService = customerService;

  public async Task<ResponseWrapper<CustomerResponse>> Handle(GetCustomerByIdQuery request, CancellationToken cancellationToken)
  {
    var customer = await _customerService.GetByIdAsync(request.Id);

    if (customer is null)
      return await ResponseWrapper<CustomerResponse>.FailAsync("Cliente nao encontrado.");

    return await ResponseWrapper<CustomerResponse>.SuccessAsync(customer.Adapt<CustomerResponse>());
  }
}

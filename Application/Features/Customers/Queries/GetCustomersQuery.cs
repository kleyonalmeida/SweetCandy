using Application.Features.Customers.DTOs;
using Application.Wrappers;
using Mapster;
using MediatR;

namespace Application.Features.Customers.Queries;

public record GetCustomersQuery(int Page = 1, int PageSize = 20) : IRequest<ResponseWrapper<List<CustomerResponse>>>;

public class GetCustomersQueryHandler(ICustomerService customerService) : IRequestHandler<GetCustomersQuery, ResponseWrapper<List<CustomerResponse>>>
{
  private readonly ICustomerService _customerService = customerService;

  public async Task<ResponseWrapper<List<CustomerResponse>>> Handle(GetCustomersQuery request, CancellationToken cancellationToken)
  {
    var customers = await _customerService.GetAllAsync();

    var projected = customers
      .Skip((request.Page - 1) * request.PageSize)
      .Take(request.PageSize)
      .Select(customer => customer.Adapt<CustomerResponse>())
      .ToList();

    return await ResponseWrapper<List<CustomerResponse>>.SuccessAsync(projected);
  }
}

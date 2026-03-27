using Application.Pipelines;
using Application.Wrappers;
using Domain.Entities;
using MediatR;

namespace Application.Features.Customers.Commands;

public class DeleteCustomerCommand(string id) : IRequest<IResponseWrapper>, IValidateMe
{
  public string Id { get; set; } = id;
}

public class DeleteCustomerCommandHandler(ICustomerService customerService) : IRequestHandler<DeleteCustomerCommand, IResponseWrapper>
{
  private readonly ICustomerService _customerService = customerService;

  public async Task<IResponseWrapper> Handle(DeleteCustomerCommand request, CancellationToken cancellationToken)
  {
    var customer = new Customer { Id = request.Id };
    var result = await _customerService.DeleteAsync(customer);

    if (!string.IsNullOrWhiteSpace(result))
      return await ResponseWrapper.FailAsync(result);

    return await ResponseWrapper.SuccessAsync("Cliente excluido com sucesso.");
  }
}

using Application.Pipelines;
using Application.Wrappers;
using MediatR;

namespace Application.Features.Budgets.Commands;

public record DeleteBudgetCommand(string Id) : IRequest<IResponseWrapper>, IValidateMe;

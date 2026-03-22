using Application.Features.Budgets.DTOs;
using Application.Pipelines;
using Application.Wrappers;
using MediatR;

namespace Application.Features.Budgets.Commands;

public record UpdateBudgetCommand(string Id, UpdateBudgetRequest UpdateBudget) : IRequest<IResponseWrapper>, IValidateMe;

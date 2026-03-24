using Application.Features.Budgets.DTOs;

namespace Application.Features.Budgets.Commands;

public record UpdateBudgetCommand(string Id, UpdateBudgetRequest UpdateBudget);

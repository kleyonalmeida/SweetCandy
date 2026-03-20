using Application.Features.Budget.DTOs;

namespace Application.Features.Budget.Commands;

public record UpdateBudgetCommand(string Id, UpdateBudgetRequest UpdateBudget);

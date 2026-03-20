namespace Application.Features.Budget.Queries;

public record GetBudgetsQuery(int Page = 1, int PageSize = 20);

namespace Application.Features.MonthlyGoals.DTOs;

public class UpsertMonthlyGoalRequest
{
  public int Year { get; set; }
  public int Month { get; set; }
  /// <summary>Valor fixo em R$. Obrigatório se PercentageOverCosts for nulo.</summary>
  public decimal? TargetAmount { get; set; }
  /// <summary>Porcentagem sobre os gastos do mês (ex.: 50 = gastos × 1.5). Obrigatório se TargetAmount for nulo.</summary>
  public decimal? PercentageOverCosts { get; set; }
}

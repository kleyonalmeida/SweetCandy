using Application.Features.Expenses;
using Application.Features.MonthlyGoals;
using Application.Features.MonthlyGoals.Commands;
using Application.Features.MonthlyGoals.DTOs;
using Domain.Entities;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Application.Tests.Features.MonthlyGoals;

public class UpsertMonthlyGoalCommandHandlerTests
{
  private readonly IMonthlyGoalService _goalService = Substitute.For<IMonthlyGoalService>();
  private readonly IExpenseService _expenseService = Substitute.For<IExpenseService>();
  private readonly UpsertMonthlyGoalCommandHandler _handler;

  public UpsertMonthlyGoalCommandHandlerTests()
  {
    _handler = new(_goalService, _expenseService);
  }

  [Fact]
  public async Task Handle_ModoValorFixo_SalvaDiretamente()
  {
    // Arrange
    var request = new UpsertMonthlyGoalRequest { Year = 2025, Month = 6, TargetAmount = 5000m };
    _goalService.UpsertAsync(Arg.Any<MonthlyGoal>()).Returns("");

    // Act
    var result = await _handler.Handle(new UpsertMonthlyGoalCommand(request), CancellationToken.None);

    // Assert
    result.IsSuccessful.Should().BeTrue();
    result.Messages.Should().Contain("Meta mensal salva com sucesso.");
    await _goalService.Received(1).UpsertAsync(Arg.Is<MonthlyGoal>(g =>
        g.TargetAmount == 5000m && g.Year == 2025 && g.Month == 6));
    await _expenseService.DidNotReceive().GetAllAsync();
  }

  [Fact]
  public async Task Handle_ModoPorcentagem_CalculaDespesas()
  {
    // Arrange — 50% sobre custos de R$1000 → 1000 × (1 + 50/100) = 1500
    var request = new UpsertMonthlyGoalRequest { Year = 2025, Month = 6, PercentageOverCosts = 50m };
    _expenseService.GetAllAsync().Returns(new List<Expense>
        {
            new() { Date = new DateTime(2025, 6, 15, 0, 0, 0, DateTimeKind.Utc), Value = 600m },
            new() { Date = new DateTime(2025, 6, 20, 0, 0, 0, DateTimeKind.Utc), Value = 400m },
            // despesa de outro mês — não deve ser somada
            new() { Date = new DateTime(2025, 5, 10, 0, 0, 0, DateTimeKind.Utc), Value = 9999m },
        });
    _goalService.UpsertAsync(Arg.Any<MonthlyGoal>()).Returns("");

    // Act
    var result = await _handler.Handle(new UpsertMonthlyGoalCommand(request), CancellationToken.None);

    // Assert
    result.IsSuccessful.Should().BeTrue();
    await _goalService.Received(1).UpsertAsync(Arg.Is<MonthlyGoal>(g => g.TargetAmount == 1500m));
  }

  [Fact]
  public async Task Handle_ModoPorcentagem_SemDespesas_SalvaZero()
  {
    // Arrange
    var request = new UpsertMonthlyGoalRequest { Year = 2025, Month = 6, PercentageOverCosts = 50m };
    _expenseService.GetAllAsync().Returns(new List<Expense>());
    _goalService.UpsertAsync(Arg.Any<MonthlyGoal>()).Returns("");

    // Act
    var result = await _handler.Handle(new UpsertMonthlyGoalCommand(request), CancellationToken.None);

    // Assert
    result.IsSuccessful.Should().BeTrue();
    await _goalService.Received(1).UpsertAsync(Arg.Is<MonthlyGoal>(g => g.TargetAmount == 0m));
  }

  [Fact]
  public async Task Handle_ErroNoService_RetornaFalha()
  {
    // Arrange
    var request = new UpsertMonthlyGoalRequest { Year = 2025, Month = 6, TargetAmount = 1000m };
    _goalService.UpsertAsync(Arg.Any<MonthlyGoal>()).Returns("Erro ao salvar meta.");

    // Act
    var result = await _handler.Handle(new UpsertMonthlyGoalCommand(request), CancellationToken.None);

    // Assert
    result.IsSuccessful.Should().BeFalse();
    result.Messages.Should().Contain("Erro ao salvar meta.");
  }

  [Fact]
  public async Task Handle_ModoPorcentagemZero_RetornaValorIgualAsDespesas()
  {
    // Arrange — 0% sobre custos → gastos × (1 + 0/100) = gastos exatos
    var request = new UpsertMonthlyGoalRequest { Year = 2025, Month = 3, PercentageOverCosts = 0m };
    _expenseService.GetAllAsync().Returns(new List<Expense>
        {
            new() { Date = new DateTime(2025, 3, 10, 0, 0, 0, DateTimeKind.Utc), Value = 800m }
        });
    _goalService.UpsertAsync(Arg.Any<MonthlyGoal>()).Returns("");

    // Act
    var result = await _handler.Handle(new UpsertMonthlyGoalCommand(request), CancellationToken.None);

    // Assert
    await _goalService.Received(1).UpsertAsync(Arg.Is<MonthlyGoal>(g => g.TargetAmount == 800m));
  }
}

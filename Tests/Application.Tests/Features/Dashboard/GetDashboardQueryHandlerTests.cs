using Application.Features.Dashboard.Queries;
using Application.Features.Expenses;
using Application.Features.MonthlyGoals;
using Application.Features.Receipts;
using Domain.Entities;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Application.Tests.Features.Dashboard;

public class GetDashboardQueryHandlerTests
{
  private readonly IReceiptsService _receiptsService = Substitute.For<IReceiptsService>();
  private readonly IExpenseService _expenseService = Substitute.For<IExpenseService>();
  private readonly IMonthlyGoalService _goalService = Substitute.For<IMonthlyGoalService>();
  private readonly GetDashboardQueryHandler _handler;

  // Mês de referência fixo para os testes (evita flakiness por virada de mês)
  private static readonly int TestYear = 2025;
  private static readonly int TestMonth = 6;

  public GetDashboardQueryHandlerTests()
  {
    _handler = new(_receiptsService, _expenseService, _goalService);
  }

  [Fact]
  public async Task Handle_ComMetaCustomizada_EffectiveGoalEhAMeta()
  {
    // Arrange
    SetupReceitasDoMes(3000m);
    SetupDespesasDoMes(1000m);
    _goalService.GetByMonthAsync(TestYear, TestMonth)
        .Returns(new MonthlyGoal { Year = TestYear, Month = TestMonth, TargetAmount = 4000m });

    // Act
    var result = await _handler.Handle(new GetDashboardQuery(TestYear, TestMonth), CancellationToken.None);

    // Assert
    result.IsSuccessful.Should().BeTrue();
    result.Data!.EffectiveGoal.Should().Be(4000m);
    result.Data.SuggestedGoal.Should().Be(1500m);     // 1000 × 1.5
    result.Data.Revenue.Should().Be(3000m);
    result.Data.Expenses.Should().Be(1000m);
    result.Data.Profit.Should().Be(2000m);
  }

  [Fact]
  public async Task Handle_SemMeta_EffectiveGoalEhMetaSugerida()
  {
    // Arrange
    SetupReceitasDoMes(0m);
    SetupDespesasDoMes(2000m);
    _goalService.GetByMonthAsync(TestYear, TestMonth).Returns((MonthlyGoal?)null);

    // Act
    var result = await _handler.Handle(new GetDashboardQuery(TestYear, TestMonth), CancellationToken.None);

    // Assert
    result.Data!.EffectiveGoal.Should().Be(3000m);    // 2000 × 1.5
    result.Data.SuggestedGoal.Should().Be(3000m);
    result.Data.MonthlyGoalTarget.Should().BeNull();
  }

  [Fact]
  public async Task Handle_SemDespesas_SuggestedGoalEhZero()
  {
    // Arrange
    SetupReceitasDoMes(0m);
    SetupDespesasDoMes(0m);
    _goalService.GetByMonthAsync(TestYear, TestMonth).Returns((MonthlyGoal?)null);

    // Act
    var result = await _handler.Handle(new GetDashboardQuery(TestYear, TestMonth), CancellationToken.None);

    // Assert
    result.Data!.SuggestedGoal.Should().Be(0m);
    result.Data.EffectiveGoal.Should().Be(0m);
    result.Data.EffectiveGoalPercent.Should().Be(0m);
  }

  [Fact]
  public async Task Handle_RecebivelMetaAtingida_PercentualMaisQue100()
  {
    // Arrange — receita > meta → percentual > 100%
    SetupReceitasDoMes(5000m);
    SetupDespesasDoMes(1000m);
    _goalService.GetByMonthAsync(TestYear, TestMonth)
        .Returns(new MonthlyGoal { Year = TestYear, Month = TestMonth, TargetAmount = 4000m });

    // Act
    var result = await _handler.Handle(new GetDashboardQuery(TestYear, TestMonth), CancellationToken.None);

    // Assert — 5000 / 4000 × 100 = 125%
    result.Data!.EffectiveGoalPercent.Should().Be(125m);
  }

  [Fact]
  public async Task Handle_DespesasDeOutroMes_NaoSaoContabilizadas()
  {
    // Arrange — despesa e receita fora do mês não devem entrar no cálculo
    _receiptsService.GetAllAsync().Returns(new List<Receipt>
        {
            new() { Date = new DateTime(TestYear, TestMonth, 15, 0, 0, 0, DateTimeKind.Utc), Amount = 1000m },
            new() { Date = new DateTime(TestYear, TestMonth - 1, 10, 0, 0, 0, DateTimeKind.Utc), Amount = 9999m },
        });
    _expenseService.GetAllAsync().Returns(new List<Expense>
        {
            new() { Date = new DateTime(TestYear, TestMonth, 5, 0, 0, 0, DateTimeKind.Utc), Value = 200m },
            new() { Date = new DateTime(TestYear, TestMonth + 1, 5, 0, 0, 0, DateTimeKind.Utc), Value = 9999m },
        });
    _goalService.GetByMonthAsync(TestYear, TestMonth).Returns((MonthlyGoal?)null);

    // Act
    var result = await _handler.Handle(new GetDashboardQuery(TestYear, TestMonth), CancellationToken.None);

    // Assert
    result.Data!.Revenue.Should().Be(1000m);
    result.Data.Expenses.Should().Be(200m);
  }

  // Helpers
  private void SetupReceitasDoMes(decimal total)
  {
    _receiptsService.GetAllAsync().Returns(total == 0m
        ? new List<Receipt>()
        : new List<Receipt>
        {
                new() { Date = new DateTime(TestYear, TestMonth, 10, 0, 0, 0, DateTimeKind.Utc), Amount = total }
        });
  }

  private void SetupDespesasDoMes(decimal total)
  {
    _expenseService.GetAllAsync().Returns(total == 0m
        ? new List<Expense>()
        : new List<Expense>
        {
                new() { Date = new DateTime(TestYear, TestMonth, 5, 0, 0, 0, DateTimeKind.Utc), Value = total }
        });
  }
}

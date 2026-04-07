using Application.Features.Categories;
using Application.Features.Expenses;
using Application.Features.Expenses.Commands;
using Application.Features.Expenses.DTOs;
using Domain.Entities;
using Domain.Enums;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Application.Tests.Features.Expenses;

public class CreateExpenseCommandHandlerTests
{
  private readonly IExpenseService _expenseService = Substitute.For<IExpenseService>();
  private readonly ICategoryService _categoryService = Substitute.For<ICategoryService>();
  private readonly CreateExpenseCommandHandler _handler;

  public CreateExpenseCommandHandlerTests()
  {
    _handler = new(_expenseService, _categoryService);
  }

  [Fact]
  public async Task Handle_CategoriaValida_RetornaSuccesso()
  {
    // Arrange
    var request = new CreateExpenseRequest
    {
      Name = "Farinha",
      Value = 50,
      CategoryId = "cat-1",
      PaymentMethod = FormaPagamento.Pix
    };
    _categoryService.GetByIdAsync("cat-1").Returns(new Category { Id = "cat-1", Name = "Insumos" });
    _expenseService.CreateAsync(Arg.Any<Expense>()).Returns(Guid.NewGuid().ToString());

    // Act
    var result = await _handler.Handle(new CreateExpenseCommand(request), CancellationToken.None);

    // Assert
    result.IsSuccessful.Should().BeTrue();
    result.Messages.Should().Contain(m => m.Contains("Despesa criada com sucesso"));
  }

  [Fact]
  public async Task Handle_CategoriaInvalida_RetornaFalha()
  {
    // Arrange
    var request = new CreateExpenseRequest
    {
      Name = "Ovos",
      Value = 30,
      CategoryId = "cat-inexistente"
    };
    _categoryService.GetByIdAsync("cat-inexistente").Returns((Category?)null);

    // Act
    var result = await _handler.Handle(new CreateExpenseCommand(request), CancellationToken.None);

    // Assert
    result.IsSuccessful.Should().BeFalse();
    result.Messages.Should().Contain("Categoria não encontrada.");
    await _expenseService.DidNotReceive().CreateAsync(Arg.Any<Expense>());
  }

  [Fact]
  public async Task Handle_SemCategoryId_PulaValidacao_RetornaSuccesso()
  {
    // Arrange — categoryId nulo ou vazio pula a validação
    var request = new CreateExpenseRequest
    {
      Name = "Embalagem",
      Value = 15,
      CategoryId = null
    };
    _expenseService.CreateAsync(Arg.Any<Expense>()).Returns(Guid.NewGuid().ToString());

    // Act
    var result = await _handler.Handle(new CreateExpenseCommand(request), CancellationToken.None);

    // Assert
    result.IsSuccessful.Should().BeTrue();
    await _categoryService.DidNotReceive().GetByIdAsync(Arg.Any<string>());
  }

  [Fact]
  public async Task Handle_CategoryIdVazio_PulaValidacao_RetornaSuccesso()
  {
    // Arrange — string vazia também pula validação (IsNullOrWhiteSpace)
    var request = new CreateExpenseRequest
    {
      Name = "Embalagem",
      Value = 15,
      CategoryId = "   "
    };
    _expenseService.CreateAsync(Arg.Any<Expense>()).Returns(Guid.NewGuid().ToString());

    // Act
    var result = await _handler.Handle(new CreateExpenseCommand(request), CancellationToken.None);

    // Assert
    result.IsSuccessful.Should().BeTrue();
    await _categoryService.DidNotReceive().GetByIdAsync(Arg.Any<string>());
  }
}

using Application.Features.Inventories;
using Application.Features.Orders;
using Application.Features.StockMovements;
using Application.Features.StockMovements.Commands;
using Application.Features.StockMovements.DTOs;
using Application.Wrappers;
using Domain.Entities;
using Domain.Enums;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Application.Tests.Features.StockMovements;

public class CreateStockMovementCommandHandlerTests
{
  private readonly IStockMovementService _stockMovementService = Substitute.For<IStockMovementService>();
  private readonly IInventoryService _inventoryService = Substitute.For<IInventoryService>();
  private readonly IOrdersService _ordersService = Substitute.For<IOrdersService>();
  private readonly CreateStockMovementCommandHandler _handler;

  public CreateStockMovementCommandHandlerTests()
  {
    _handler = new(_stockMovementService, _inventoryService, _ordersService);
  }

  [Fact]
  public async Task Handle_InsumoValido_RetornaSuccesso()
  {
    // Arrange
    var request = new CreateStockMovementRequest
    {
      SupplyId = "supply-1",
      Quantity = 5,
      Type = MovementType.Entrada
    };
    var newId = Guid.NewGuid().ToString();
    _inventoryService.GetSupplyByIdAsync("supply-1").Returns(new Supply { Id = "supply-1" });
    _stockMovementService.CreateAsync(Arg.Any<StockMovement>()).Returns(newId);

    // Act
    var result = await _handler.Handle(new CreateStockMovementCommand(request), CancellationToken.None);

    // Assert
    result.IsSuccessful.Should().BeTrue();
    result.Messages.Should().Contain(m => m.Contains(newId));
  }

  [Fact]
  public async Task Handle_InsumoNaoEncontrado_RetornaFalha()
  {
    // Arrange
    var request = new CreateStockMovementRequest
    {
      SupplyId = "insumo-invalido",
      Quantity = 5,
      Type = MovementType.Entrada
    };
    _inventoryService.GetSupplyByIdAsync("insumo-invalido").Returns((Supply?)null);

    // Act
    var result = await _handler.Handle(new CreateStockMovementCommand(request), CancellationToken.None);

    // Assert
    result.IsSuccessful.Should().BeFalse();
    result.Messages.Should().Contain("Insumo nao encontrado.");
    await _stockMovementService.DidNotReceive().CreateAsync(Arg.Any<StockMovement>());
  }

  [Fact]
  public async Task Handle_EstoqueInsuficiente_RetornaFalha()
  {
    // Arrange — o serviço retorna mensagem de erro (não é GUID)
    var request = new CreateStockMovementRequest
    {
      SupplyId = "supply-1",
      Quantity = 100,
      Type = MovementType.Saida
    };
    _inventoryService.GetSupplyByIdAsync("supply-1").Returns(new Supply { Id = "supply-1" });
    _stockMovementService.CreateAsync(Arg.Any<StockMovement>())
        .Returns("Estoque insuficiente. Disponível: 5, solicitado: 100.");

    // Act
    var result = await _handler.Handle(new CreateStockMovementCommand(request), CancellationToken.None);

    // Assert
    result.IsSuccessful.Should().BeFalse();
    result.Messages.Should().Contain(m => m.Contains("Estoque insuficiente"));
  }

  [Fact]
  public async Task Handle_SemSupplyId_NaoValidaInsumo_RetornaSuccesso()
  {
    // Arrange — sem SupplyId, pula a validação de insumo
    var request = new CreateStockMovementRequest
    {
      SupplyId = null,
      Quantity = 3,
      Type = MovementType.Entrada
    };
    _stockMovementService.CreateAsync(Arg.Any<StockMovement>()).Returns(Guid.NewGuid().ToString());

    // Act
    var result = await _handler.Handle(new CreateStockMovementCommand(request), CancellationToken.None);

    // Assert
    result.IsSuccessful.Should().BeTrue();
    await _inventoryService.DidNotReceive().GetSupplyByIdAsync(Arg.Any<string>());
  }

  [Fact]
  public async Task Handle_PedidoNaoEncontrado_RetornaFalha()
  {
    // Arrange
    var request = new CreateStockMovementRequest
    {
      SupplyId = null,
      OrderId = "pedido-invalido",
      Quantity = 2,
      Type = MovementType.Saida
    };
    _ordersService.GetByIdAsync("pedido-invalido").Returns((Order?)null);

    // Act
    var result = await _handler.Handle(new CreateStockMovementCommand(request), CancellationToken.None);

    // Assert
    result.IsSuccessful.Should().BeFalse();
    result.Messages.Should().Contain("Pedido nao encontrado.");
  }
}

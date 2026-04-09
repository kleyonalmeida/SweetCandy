using Application.Features.Orders;
using Application.Features.Orders.Commands;
using Application.Features.Orders.DTOs;
using Application.Features.Receipts;
using Application.Features.StockMovements;
using Domain.Entities;
using Domain.Enums;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Application.Tests.Features.Orders;

public class UpdateOrderCommandHandlerTests
{
    private readonly IOrdersService _ordersService = Substitute.For<IOrdersService>();
    private readonly IStockMovementService _stockMovementService = Substitute.For<IStockMovementService>();
    private readonly IReceiptsService _receiptsService = Substitute.For<IReceiptsService>();
    private readonly UpdateOrderCommandHandler _handler;

    public UpdateOrderCommandHandlerTests()
    {
        _handler = new(_ordersService, _stockMovementService, _receiptsService);
    }

    [Fact]
    public async Task Handle_PedidoNaoEncontrado_RetornaFalha()
    {
        // Arrange
        _ordersService.GetByIdAsync("id-inexistente").Returns((Order?)null);

        // Act
        var result = await _handler.Handle(
            new UpdateOrderCommand("id-inexistente", new UpdateOrderRequest()),
            CancellationToken.None);

        // Assert
        result.IsSuccessful.Should().BeFalse();
        result.Messages.Should().Contain("Pedido nao encontrado.");
    }

    [Fact]
    public async Task Handle_TransicaoPendenteParaConfirmada_CriaMovimentacaoDeEstoque()
    {
        // Arrange
        var order = BuildOrder(StatusOrder.Pendente, 500m, new List<OrderItem>
        {
            new() { FinalProductId = "fp-1", Quantity = 2 }
        });
        _ordersService.GetByIdAsync(order.Id).Returns(order);
        _ordersService.UpdateAsync(Arg.Any<Order>()).Returns("");
        _stockMovementService.CreateAsync(Arg.Any<StockMovement>()).Returns(Guid.NewGuid().ToString());

        // Act
        var result = await _handler.Handle(
            new UpdateOrderCommand(order.Id, new UpdateOrderRequest { Status = StatusOrder.Confirmada }),
            CancellationToken.None);

        // Assert
        result.IsSuccessful.Should().BeTrue();
        await _stockMovementService.Received(1).CreateAsync(Arg.Is<StockMovement>(m =>
            m.SupplyId == "fp-1" && m.Quantity == 2 && m.Type == MovementType.Saida));
    }

    [Fact]
    public async Task Handle_TransicaoConfirmada_EstoqueInsuficiente_RetornaFalha()
    {
        // Arrange
        var order = BuildOrder(StatusOrder.Pendente, 200m, new List<OrderItem>
        {
            new() { FinalProductId = "fp-1", Quantity = 999 }
        });
        _ordersService.GetByIdAsync(order.Id).Returns(order);
        _ordersService.UpdateAsync(Arg.Any<Order>()).Returns("");
        _stockMovementService.CreateAsync(Arg.Any<StockMovement>())
            .Returns("Estoque insuficiente. Disponível: 0, solicitado: 999.");

        // Act
        var result = await _handler.Handle(
            new UpdateOrderCommand(order.Id, new UpdateOrderRequest { Status = StatusOrder.Confirmada }),
            CancellationToken.None);

        // Assert
        result.IsSuccessful.Should().BeFalse();
        result.Messages.Should().Contain(m => m.Contains("erro(s) na baixa de estoque"));
    }

    [Fact]
    public async Task Handle_TransicaoPendenteParaConcluida_CriaReceita()
    {
        // Arrange
        var order = BuildOrder(StatusOrder.Pendente, 800m, new List<OrderItem>());
        _ordersService.GetByIdAsync(order.Id).Returns(order);
        _ordersService.UpdateAsync(Arg.Any<Order>()).Returns("");
        _receiptsService.CreateAsync(Arg.Any<Receipt>()).Returns(Guid.NewGuid().ToString());

        // Act
        var result = await _handler.Handle(
            new UpdateOrderCommand(order.Id, new UpdateOrderRequest { Status = StatusOrder.Concluida }),
            CancellationToken.None);

        // Assert
        result.IsSuccessful.Should().BeTrue();
        await _receiptsService.Received(1).CreateAsync(Arg.Is<Receipt>(r =>
            r.Amount == 800m && r.OrderId == order.Id && r.CustomerId == order.CustomerId));
    }

    [Fact]
    public async Task Handle_JaEstavaConcluida_NaoCriaReceita()
    {
        // Arrange — status não muda, não deve criar nova receita
        var order = BuildOrder(StatusOrder.Concluida, 500m, new List<OrderItem>());
        _ordersService.GetByIdAsync(order.Id).Returns(order);
        _ordersService.UpdateAsync(Arg.Any<Order>()).Returns("");

        // Act
        var result = await _handler.Handle(
            new UpdateOrderCommand(order.Id, new UpdateOrderRequest { Status = StatusOrder.Concluida }),
            CancellationToken.None);

        // Assert
        result.IsSuccessful.Should().BeTrue();
        await _receiptsService.DidNotReceive().CreateAsync(Arg.Any<Receipt>());
    }

    [Fact]
    public async Task Handle_JaEraConfirmada_NaoCriaNovaBaixaDeEstoque()
    {
        // Arrange — pedido já estava Confirmado, atualiza apenas o nome
        var order = BuildOrder(StatusOrder.Confirmada, 300m, new List<OrderItem>
        {
            new() { FinalProductId = "fp-1", Quantity = 1 }
        });
        _ordersService.GetByIdAsync(order.Id).Returns(order);
        _ordersService.UpdateAsync(Arg.Any<Order>()).Returns("");

        // Act
        var result = await _handler.Handle(
            new UpdateOrderCommand(order.Id, new UpdateOrderRequest { Name = "Novo Nome" }),
            CancellationToken.None);

        // Assert
        result.IsSuccessful.Should().BeTrue();
        await _stockMovementService.DidNotReceive().CreateAsync(Arg.Any<StockMovement>());
    }

    [Fact]
    public async Task Handle_MultiplosProdutos_CriaUmMovimentoPorItem()
    {
        // Arrange
        var order = BuildOrder(StatusOrder.Pendente, 600m, new List<OrderItem>
        {
            new() { FinalProductId = "fp-1", Quantity = 1 },
            new() { FinalProductId = "fp-2", Quantity = 3 },
            new() { FinalProductId = "fp-3", Quantity = 2 },
        });
        _ordersService.GetByIdAsync(order.Id).Returns(order);
        _ordersService.UpdateAsync(Arg.Any<Order>()).Returns("");
        _stockMovementService.CreateAsync(Arg.Any<StockMovement>()).Returns(Guid.NewGuid().ToString());

        // Act
        var result = await _handler.Handle(
            new UpdateOrderCommand(order.Id, new UpdateOrderRequest { Status = StatusOrder.Confirmada }),
            CancellationToken.None);

        // Assert
        result.IsSuccessful.Should().BeTrue();
        await _stockMovementService.Received(3).CreateAsync(Arg.Any<StockMovement>());
    }

    // Helper
    private static Order BuildOrder(StatusOrder status, decimal totalValue, List<OrderItem> items)
    {
        var order = new Order
        {
            Name = "Pedido Teste",
            Status = status,
            TotalValue = totalValue,
            CustomerId = "customer-1",
        };
        order.SetItems(items);
        return order;
    }
}

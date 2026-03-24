using Application.Features.Budgets;
using Application.Features.Budgets.Commands;
using Application.Features.Inventories;
using Application.Features.Orders;
using Application.Features.Receipts;
using Application.Pipelines;
using FluentValidation;
using Infrastructure.Budgets;
using Infrastructure.Inventories;
using Infrastructure.Orders;
using Infrastructure.Receipts;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateBudgetCommand).Assembly));
builder.Services.AddValidatorsFromAssembly(typeof(CreateBudgetCommand).Assembly);
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBenaviour<,>));

builder.Services.AddSingleton<IBudgetService, BudgetService>();
builder.Services.AddSingleton<IInventoryService, InventoryService>();
builder.Services.AddSingleton<IOrdersService, OrderService>();
builder.Services.AddSingleton<IReceiptsService, ReceiptService>();

var app = builder.Build();

app.MapControllers();
app.MapGet("/", () => Results.Ok(new { message = "SweetCandy API online" }));

app.Run();

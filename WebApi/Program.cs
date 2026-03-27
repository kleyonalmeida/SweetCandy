using Application.Features.Budgets;
using Application.Features.Budgets.Commands;
using Application.Features.Customers;
using Application.Features.Expenses;
using Application.Features.Inventories;
using Application.Features.MonthlyGoals;
using Application.Features.Orders;
using Application.Features.Receipts;
using Application.Pipelines;
using FluentValidation;
using Infrastructure.Budgets;
using Infrastructure.Customers;
using Infrastructure.Expenses;
using Infrastructure.Inventories;
using Infrastructure.MonthlyGoals;
using Infrastructure.Orders;
using Infrastructure.Receipts;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
  options.AddPolicy("Frontend", builder =>
  {
    builder
      .AllowAnyMethod()
      .AllowAnyHeader()
      .WithOrigins("http://localhost:5173", "http://localhost:3000");
  });
});

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateBudgetCommand).Assembly));
builder.Services.AddValidatorsFromAssembly(typeof(CreateBudgetCommand).Assembly);
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBenaviour<,>));

builder.Services.AddSingleton<IBudgetService, BudgetService>();
builder.Services.AddSingleton<IInventoryService, InventoryService>();
builder.Services.AddSingleton<IOrdersService, OrderService>();
builder.Services.AddSingleton<IReceiptsService, ReceiptService>();
builder.Services.AddSingleton<ICustomerService, CustomerService>();
builder.Services.AddSingleton<IExpenseService, ExpenseService>();
builder.Services.AddSingleton<IMonthlyGoalService, MonthlyGoalService>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("Frontend");

app.MapControllers();
app.MapGet("/", () => Results.Ok(new { message = "SweetCandy API online" }));

app.Run();

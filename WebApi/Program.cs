using Application.Features.Budgets;
using Application.Features.Budgets.Commands;
using Application.Features.Categories;
using Application.Features.Customers;
using Application.Features.Expenses;
using Application.Features.Inventories;
using Application.Features.Orders;
using Application.Features.Receipts;
using Application.Features.StockMovements;
using Application.Pipelines;
using FluentValidation;
using Infrastructure.Budgets;
using Infrastructure.Categories;
using Infrastructure.Customers;
using Infrastructure.Expenses;
using Infrastructure.Inventories;
using Infrastructure.Orders;
using Infrastructure.Persistence;
using Infrastructure.Receipts;
using Infrastructure.StockMovements;
using MediatR;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
  options.AddDefaultPolicy(policy =>
  {
    policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
  });
});

var dbPath = Path.Combine(builder.Environment.ContentRootPath, "sweetcandy.db");
builder.Services.AddDbContext<AppDbContext>(options =>
  options.UseSqlite($"Data Source={dbPath}"));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateBudgetCommand).Assembly));
builder.Services.AddValidatorsFromAssembly(typeof(CreateBudgetCommand).Assembly);
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBenaviour<,>));
builder.Services.AddScoped<IBudgetService, BudgetService>();
builder.Services.AddScoped<IInventoryService, InventoryService>();
builder.Services.AddScoped<IOrdersService, OrderService>();
builder.Services.AddScoped<IReceiptsService, ReceiptService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IExpenseService, ExpenseService>();
builder.Services.AddScoped<IStockMovementService, StockMovementService>();

var app = builder.Build();

// Aplica migrations automaticamente na inicialização
using (var scope = app.Services.CreateScope())
{
  var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
  db.Database.Migrate();
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors();
app.UseDefaultFiles();
app.UseStaticFiles();

app.MapControllers();

app.Run();

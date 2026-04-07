using Application.Features.Categories;
using Application.Features.Expenses.DTOs;
using Application.Pipelines;
using Application.Wrappers;
using Domain.Entities;
using Mapster;
using MediatR;

namespace Application.Features.Expenses.Commands;

public class CreateExpenseCommand(CreateExpenseRequest createExpense) : IRequest<IResponseWrapper>, IValidateMe
{
  public CreateExpenseRequest CreateExpense { get; set; } = createExpense;
}

public class CreateExpenseCommandHandler(
  IExpenseService expenseService,
  ICategoryService categoryService) : IRequestHandler<CreateExpenseCommand, IResponseWrapper>
{
  private readonly IExpenseService _expenseService = expenseService;
  private readonly ICategoryService _categoryService = categoryService;

  public async Task<IResponseWrapper> Handle(CreateExpenseCommand request, CancellationToken cancellationToken)
  {
    if (!string.IsNullOrWhiteSpace(request.CreateExpense.CategoryId))
    {
      var category = await _categoryService.GetByIdAsync(request.CreateExpense.CategoryId);
      if (category is null)
        return await ResponseWrapper.FailAsync("Categoria não encontrada.");
    }

    var expense = request.CreateExpense.Adapt<Expense>();

    var createdExpenseId = await _expenseService.CreateAsync(expense);

    return await ResponseWrapper.SuccessAsync($"Despesa criada com sucesso. Id: {createdExpenseId}");
  }
}

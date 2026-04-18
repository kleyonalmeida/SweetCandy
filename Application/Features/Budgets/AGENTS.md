# AGENTS.md — Application/Features/Budgets/

Feature `Budgets` — orçamentos e conversão para pedido.

Arquivos na raiz da feature:
- IBudgetService.cs

Subpastas e responsabilidades:
- Commands/: Create/Update/Delete/Convert commands
- DTOs/: Requests e Responses de Budget e BudgetItem
- Queries/: Listagem e obtenção por id
- Validations/: FluentValidation para cada command

Contrato/Comportamento principal:
- `CreateBudgetCommand` valida cliente e itens, calcula `FinalTotalValue`.
- `ConvertBudgetToOrderCommand` cria `Order` copiando itens e cliente do `Budget`.

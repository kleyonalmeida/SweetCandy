# Modificações recentes no backend (SweetCandy)

Documento descreve o que foi alterado ou adicionado e o motivo, alinhado ao produto SaaS de gestão para confeiteiros (API consumível pelo frontend e pelo Swagger).

---

## Contexto

O backend já seguia Clean Architecture (Domain → Application com MediatR/FluentValidation/Mapster → Infrastructure in-memory → WebApi). Faltavam recursos para **finanças**, **clientes**, **painel**, **meta mensal**, **ajuste de estoque** e **documentação HTTP estável** para desenvolvimento.

---

## Swagger e pacote Swashbuckle

**O quê:** Referência explícita ao pacote `Swashbuckle.AspNetCore` (6.6.2) no projeto `WebApi` e uso de `AddSwaggerGen()`, `UseSwagger()` e `UseSwaggerUI()` em `Program.cs` (configuração padrão, sem título/OpenAPI customizado).

**Por quê:** Garantir que a geração da especificação OpenAPI e a UI do Swagger funcionem de forma previsível ao restaurar pacotes NuGet, e facilitar testes manuais dos endpoints durante o desenvolvimento do frontend.

---

## CORS

**O quê:** Política `Frontend` com origens `http://localhost:5173` (Vite) e `http://localhost:3000`, métodos e headers permitidos.

**Por quê:** O navegador bloqueia chamadas da SPA para outra origem sem CORS; essas portas são as mais comuns para React em desenvolvimento.

---

## Feature Clientes (`Customer`)

**O quê:**

- `Infrastructure/Customers/CustomerService.cs` — persistência em memória alinhada às outras features.
- `WebApi/Controllers/CustomersController.cs` — CRUD REST (`api/customers`).
- `Application/Features/Customers/Validations/DeleteCustomerCommandValidator.cs` — validação de exclusão coerente com o pipeline FluentValidation.

**Por quê:** A entidade `Customer` existia no domínio e a camada Application já tinha commands/queries, mas faltavam serviço, DI e API. Pedidos e orçamentos no produto dependem de clientes cadastrados.

---

## Feature Despesas (`Expense`)

**O quê:**

- Entidade `Expense` ampliada com `Date` e `Category` (texto), para filtro por período e agrupamento na tela de Finanças.
- `Infrastructure/Expenses/ExpenseService.cs`.
- Commands, queries, validators e `ExpensesController` (lista com query opcional `from` / `to`).

**Por quê:** Completar o modelo de custos do negócio e alimentar o painel (despesas do mês) e a visão financeira; antes só existia a entidade, sem Application/Infrastructure/HTTP.

---

## Estoque — ajuste manual e histórico (`StockMovement`)

**O quê:**

- Extensão de `IInventoryService` / `InventoryService`: ajuste de quantidade por insumo (`Add` = entrada / saída), registro de `StockMovement`, listagem com filtro opcional por `supplyId`.
- `AdjustSupplyStockCommand`, DTOs de request/response, `GetStockMovementsQuery` mapeando para `StockMovementResponse`.
- Rotas em `InventoriesController`:
  - `POST api/inventories/supplies/{id}/adjustments`
  - `GET api/inventories/stock-movements`

**Por quê:** Atender o fluxo de “adicionar/reduzir estoque com preview e histórico” descrito para o produto. A validação principal do ajuste fica no handler (e no serviço) em vez do pipeline `IValidateMe`, para evitar conflito de tipos com `ResponseWrapper<T>` na validação global do MediatR.

---

## Meta mensal (`MonthlyGoal`)

**O quê:**

- Nova entidade `MonthlyGoal` (`Year`, `Month`, `TargetAmount`).
- `MonthlyGoalService`, `UpsertMonthlyGoalCommand`, `GetMonthlyGoalByMonthQuery`, `MonthlyGoalsController`.

**Por quê:** Suportar a barra de progresso da meta no painel; uma meta por par ano/mês, com upsert idempotente para o mesmo mês.

---

## Painel — agregador (`Dashboard`)

**O quê:**

- `GetDashboardQuery` e `DashboardController` (`GET api/dashboard?year=&month=`).
- Cálculo: receitas do mês (`Receipt`), despesas do mês (`Expense`), lucro, meta do mês e `GoalProgressPercent` (receita ÷ meta × 100 quando há meta e valor positivo).

**Por quê:** Centralizar em um endpoint os números da tela inicial sem o frontend somar várias listas; reduz acoplamento e tráfego.

---

## Injeção de dependências (`Program.cs`)

**O quê:** Registro de `ICustomerService`, `IExpenseService`, `IMonthlyGoalService` com as implementações em Infrastructure, e `using`s necessários.

**Por quê:** Completar o wiring da aplicação para os novos serviços e controllers.

---

## Resumo rápido de endpoints novos ou relevantes

| Área        | Método | Rota (prefixo `api/`) |
|------------|--------|------------------------|
| Clientes   | CRUD   | `customers`            |
| Despesas   | CRUD   | `expenses` (GET com `from`/`to` opcionais) |
| Estoque    | POST   | `inventories/supplies/{id}/adjustments` |
| Estoque    | GET    | `inventories/stock-movements` |
| Meta       | GET    | `monthly-goals?year=&month=` |
| Meta       | POST   | `monthly-goals`        |
| Painel     | GET    | `dashboard?year=&month=` |

---

## Observações

- Dados continuam **em memória** (reinício do processo apaga tudo), coerente com o restante da Infrastructure atual.
- O Swagger está ativo em **qualquer ambiente** no `Program.cs` atual; em produção costuma-se restringir à fase de desenvolvimento, se desejarem.

---

*Última atualização: documento gerado para refletir o estado do repositório após as entregas descritas acima.*

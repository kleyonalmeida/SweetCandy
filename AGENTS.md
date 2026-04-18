# AGENTS.md — SweetCandy

## Visão Geral

**SweetCandy** é um sistema de gestão para confeitaria/doceria, desenvolvido como um **monólito modular** com foco em controle de estoque, pedidos, orçamentos, despesas e metas financeiras.

---

## Stack Principal

| Camada | Tecnologia |
|---|---|
| Backend | .NET 8 + ASP.NET Core |
| Persistência | EF Core 8 + SQLite |
| CQRS | MediatR 14.1.0 |
| Validação | FluentValidation 12.1.1 |
| Mapeamento | Mapster 7.4.0 |
| Documentação API | Swashbuckle (Swagger) 6.9.0 |
| Testes | xUnit + NSubstitute + FluentAssertions + coverlet |
| Frontend | SPA estática servida pelo próprio WebApi via `wwwroot/` |

---

## Arquitetura

O projeto segue **Clean Architecture** organizado em 4 camadas com dependências unidirecionais:

```
Domain → Application → Infrastructure → WebApi
```

### Projetos da solução (`SweetCandy.sln`)

| Projeto | Responsabilidade |
|---|---|
| `Domain` | Entidades, enums, regras de negócio puras |
| `Application` | CQRS (commands/queries), validações, mapeamentos, contratos |
| `Infrastructure` | Persistência (EF Core + SQLite), implementação dos serviços |
| `WebApi` | Controllers HTTP, DI, configuração, Swagger |
| `Tests/Application.Tests` | Testes unitários dos handlers |

---

## Princípios Obrigatórios

### 1. TDD (Test Driven Development)

Este projeto **deve seguir TDD** obrigatoriamente.

**Ciclo:**
1. Escrever o teste (falhando — red)
2. Implementar o mínimo para passar (green)
3. Refatorar

**Tipos de testes obrigatórios:**
- **Unitários** — handlers de Application (Domain)
- **Integração** — Infrastructure + banco real (SQLite in-memory)
- **API** — Controllers (WebApi)

### 2. Padrão CQRS com MediatR

Todo caso de uso é implementado como **Command** ou **Query** no projeto `Application`.

- Commands que precisam de validação implementam `IValidateMe` (interface marcadora)
- O `ValidationPipelineBehaviour` intercepta automaticamente e executa todos os `IValidator<T>` registrados
- Handlers retornam sempre `IResponseWrapper<T>` ou `IResponseWrapper`

### 3. ResponseWrapper padronizado

Toda resposta de handler deve usar `ResponseWrapper`:

```csharp
// Sucesso
ResponseWrapper<T>.SuccessAsync(data)
ResponseWrapper.SuccessAsync(messages)

// Falha
ResponseWrapper.FailAsync(errors)
ResponseWrapper<T>.FailAsync(errors)
```

O `ApiControllerBase` mapeia automaticamente: `IsSuccessful = true` → `200 OK`; `false` → `400 Bad Request`.

### 4. Separação de responsabilidades

- **Domain** não referencia nenhuma outra camada
- **Application** define interfaces (`IXyzService`) que são implementadas em Infrastructure
- **Infrastructure** não é referenciada por Application (inversão de dependência)
- **WebApi** é o ponto de composição (DI root)

---

## Domain

### Entidade Base

```csharp
BaseEntity {
    Id: string (GUID)
    CreatedAt: DateTime
    UpdatedAt: DateTime
    MarkUpdated()
}
```

### Entidades

| Entidade | Propriedades-chave | Relacionamentos |
|---|---|---|
| `Budget` | `ClientName`, `CustomerId`, `EventDate`, `FinalProductName`, `FinalUnitPrice`, `FinalTotalValue` | → `Customer`, → `BudgetItem[]` (cascata) |
| `BudgetItem` | `FinalProductId`, `FinalProductName`, `Quantity`, `UnitPrice`, `TotalPrice` (calculado) | → `Budget`, → `FinalProduct` |
| `Category` | `Name`, `Description` | → `FinalProduct[]` |
| `Customer` | `Name`, `Email`, `Phone`, `Address`, `BirthDate` | → `Budget[]`, → `Order[]` |
| `Expense` | `Name`, `Value`, `Paid`, `Date`, `CategoryId`, `PaymentMethod` | → `Category` (set null) |
| `FinalProduct` | `Name`, `Description`, `CategoryId`, `CostPrice`, `UnitPrice`, `QuantityAvailable` | → `Category`, → `RecipeItem[]` (cascata) |
| `Inventory` | `TotalInvested` | → `Supply[]` |
| `MonthlyGoal` | `Year`, `Month`, `TargetAmount` | — |
| `Order` | `Name`, `CustomerId`, `EventDate`, `Status`, `Sinal`, `TotalValue` | → `Customer`, → `OrderItem[]` (cascata) |
| `OrderItem` | `FinalProductId`, `FinalProductName`, `Quantity`, `UnitPrice`, `TotalPrice` (calculado) | → `Order`, → `FinalProduct` |
| `Receipt` | `Date`, `FinalProductName`, `Amount`, `Description`, `PaymentMethod`, `OrderId`, `CustomerId` | → `Order`, → `Customer` |
| `RecipeItem` | `FinalProductId`, `SupplyId`, `Quantity`, `Unit` | → `FinalProduct`, → `Supply` |
| `StockMovement` | `Date`, `SupplyId`, `Quantity`, `Type`, `OrderId`, `Notes` | → `Supply`, → `Order` |
| `Supply` | `Name`, `Quantity`, `Price`, `Unit`, `InventoryId` | → `Inventory` |

### Enums

| Enum | Valores |
|---|---|
| `FormaPagamento` | `Dinheiro`, `Debito`, `Credito`, `Pix` |
| `MovementType` | `Entrada`, `Saida` |
| `StatusOrder` | `Pendente`, `Confirmada`, `Cancelada`, `Concluida` |
| `Unidade` | `Un`, `Kg`, `G`, `L`, `Ml`, `Mg`, `Caixa`, `Pacote` |

---

## Application

### Pipelines

- **`IValidateMe`** — interface marcadora; aplicar em Commands que precisam de validação automática
- **`ValidationPipelineBehaviour<TRequest, TResponse>`** — executa todos os `IValidator<TRequest>` em paralelo antes de chegar ao handler; retorna `ResponseWrapper.FailAsync(errors)` se houver falhas

### Wrappers

- **`IResponseWrapper`** — `{ Messages: List<string>; IsSuccessful: bool }`
- **`IResponseWrapper<T>`** — adiciona `Data: T?`
- **`ResponseWrapper` / `ResponseWrapper<T>`** — implementações concretas com métodos estáticos `Fail`, `Success` e variantes `Async`

### Common

- **`MapsterSettings`** — configuração global: `IgnoreNullValues(true)`, trim automático em strings

### Features e CQRS

#### Budgets

| Tipo | Nome | Descrição |
|---|---|---|
| Command | `CreateBudgetCommand` | Cria orçamento validando cliente e produtos; calcula `FinalTotalValue` automaticamente |
| Command | `UpdateBudgetCommand` | Atualiza campos e recria itens |
| Command | `DeleteBudgetCommand` | Remove orçamento |
| Command | `ConvertBudgetToOrderCommand` | Converte orçamento em pedido copiando itens e cliente |
| Query | `GetBudgetsQuery(page, pageSize)` | Lista paginada |
| Query | `GetBudgetByIdQuery(id)` | Retorna orçamento com itens |

#### Categories

| Tipo | Nomes |
|---|---|
| Commands | `CreateCategoryCommand`, `UpdateCategoryCommand`, `DeleteCategoryCommand` |
| Queries | `GetCategoriesQuery(page, pageSize)`, `GetCategoryByIdQuery(id)` |

#### Customers

| Tipo | Nomes |
|---|---|
| Commands | `CreateCustomerCommand`, `UpdateCustomerCommand`, `DeleteCustomerCommand` |
| Queries | `GetCustomersQuery(page, pageSize)`, `GetCustomerByIdQuery(id)` |

#### Dashboard

| Query | `GetDashboardQuery(year?, month?)` |
|---|---|
| Retorna | `Revenue`, `Expenses`, `Profit`, `GoalProgressPercent`, `SuggestedGoal` (despesas × 1,5), `EffectiveGoal`, `EffectiveGoalPercent` do mês |

#### Expenses

| Tipo | Nomes |
|---|---|
| Commands | `CreateExpenseCommand` (valida `CategoryId`), `UpdateExpenseCommand`, `DeleteExpenseCommand` |
| Queries | `GetExpensesQuery(page, pageSize, from?, to?)`, `GetExpenseByIdQuery(id)` |

#### Inventories (Supplies + FinalProducts + RecipeItems + StockMovements)

| Tipo | Nome | Descrição |
|---|---|---|
| Command | `CreateSupplyCommand` | Cadastra insumo no inventário |
| Command | `UpdateSupplyCommand` | Atualiza dados do insumo |
| Command | `DeleteSupplyCommand` | Remove insumo |
| Command | `AdjustSupplyStockCommand` | Ajusta estoque e registra `StockMovement` atomicamente |
| Command | `CreateFinalProductCommand` | Cadastra produto final (valida categoria) |
| Command | `UpdateFinalProductCommand` | Atualiza produto final |
| Command | `DeleteFinalProductCommand` | Remove produto final |
| Command | `AddRecipeItemCommand` | Adiciona item à receita do produto |
| Command | `RemoveRecipeItemCommand` | Remove item da receita |
| Query | `GetInventoryQuery()` | Retorna inventário único (auto-criado se inexistente) |
| Query | `GetSuppliesQuery(page, pageSize)` | Lista paginada de insumos |
| Query | `GetSupplyByIdQuery(id)` | Retorna insumo |
| Query | `GetFinalProductsQuery(page, pageSize)` | Lista paginada de produtos finais |
| Query | `GetFinalProductByIdQuery(id)` | Retorna produto final |
| Query | `GetRecipeQuery(id)` | Retorna receita do produto |
| Query | `GetStockMovementsQuery(page, pageSize)` | Lista paginada de movimentações |

#### MonthlyGoals

| Tipo | Nome | Descrição |
|---|---|---|
| Command | `UpsertMonthlyGoalCommand` | Cria ou atualiza meta — suporta valor fixo (`TargetAmount`) ou percentual sobre despesas (`PercentageOverCosts`) |
| Query | `GetMonthlyGoalByMonthQuery(year, month)` | Retorna meta do mês |

#### Orders

| Tipo | Nomes |
|---|---|
| Commands | `CreateOrderCommand` (valida cliente e produtos, calcula total), `UpdateOrderCommand`, `DeleteOrderCommand` |
| Queries | `GetOrdersQuery(page, pageSize)`, `GetOrderByIdQuery(id)` |

#### Receipts

| Tipo | Nomes |
|---|---|
| Commands | `CreateReceiptCommand` (valida pedido e cliente), `UpdateReceiptCommand`, `DeleteReceiptCommand` |
| Queries | `GetReceiptsQuery(page, pageSize)`, `GetReceiptByIdQuery(id)` |

#### StockMovements

| Tipo | Nome | Descrição |
|---|---|---|
| Command | `CreateStockMovementCommand` | Valida insumo e pedido; bloqueia saída com estoque insuficiente |
| Query | `GetStockMovementsQuery(page, pageSize)` | Lista paginada |
| Query | `GetStockMovementsBySupplyIdQuery(supplyId)` | Movimentações por insumo |
| Query | `GetStockMovementsByOrderIdQuery(orderId)` | Movimentações por pedido |

---

## Infrastructure

### Banco de Dados

- **SQLite** via `EF Core 8`
- Arquivo: `sweetcandy.db` (relativo ao `ContentRootPath`)
- Migrations aplicadas automaticamente no startup (`db.Database.Migrate()`)

### Migrations

| Migration | Descrição |
|---|---|
| `20260401184407_InitialCreate` | Schema inicial completo |
| `20260402142854_AddPaymentMethodToExpense` | Colunas `Date`, `Category`, `PaymentMethod` em Expenses; tabela `MonthlyGoals` |
| `20260407175149_AddExpenseCategoryFK` | FK `CategoryId` em Expenses |

### Serviços (Infrastructure → Application)

| Serviço | Interface |
|---|---|
| `BudgetService` | `IBudgetService` |
| `CategoryService` | `ICategoryService` |
| `CustomerService` | `ICustomerService` |
| `ExpenseService` | `IExpenseService` |
| `InventoryService` | `IInventoryService` |
| `MonthlyGoalService` | `IMonthlyGoalService` |
| `OrderService` | `IOrdersService` |
| `ReceiptService` | `IReceiptsService` |
| `StockMovementService` | `IStockMovementService` |

**Comportamento especial de `StockMovementService`:**
- Em movimentos do tipo `Saida`, valida se `Supply.Quantity >= quantidade solicitada`; retorna erro se insuficiente
- Atualiza `Supply.Quantity` atomicamente junto com a criação do `StockMovement`

**Comportamento especial de `InventoryService`:**
- `GetInventoryAsync()` auto-cria o inventário único se não existir

---

## WebApi

### Configuração (`Program.cs`)

- CORS: `AllowAnyOrigin / AllowAnyMethod / AllowAnyHeader` (ambiente de desenvolvimento)
- Swagger sempre habilitado
- MediatR e FluentValidation registrados do assembly `Application`
- `ValidationPipelineBehaviour` registrado como `IPipelineBehavior` transient
- Todos os serviços registrados como **Scoped**
- Arquivos estáticos servidos via `UseDefaultFiles` + `UseStaticFiles`

### Controllers e Endpoints

#### `BudgetsController` — `/api/Budgets`

| Método HTTP | Rota | CQRS |
|---|---|---|
| GET | `GetAll?page&pageSize` | `GetBudgetsQuery` |
| GET | `GetById/{id}` | `GetBudgetByIdQuery` |
| POST | `Create` | `CreateBudgetCommand` |
| PUT | `Update/{id}` | `UpdateBudgetCommand` |
| DELETE | `Delete/{id}` | `DeleteBudgetCommand` |
| POST | `ConvertToOrder/{id}` | `ConvertBudgetToOrderCommand` |

#### `CategoriesController` — `/api/Categories`

| Método HTTP | Rota |
|---|---|
| GET | `GetAll?page&pageSize` |
| GET | `GetById/{id}` |
| POST | `Create` |
| PUT | `Update/{id}` |
| DELETE | `Delete/{id}` |

#### `CustomersController` — `/api/Customers`

| Método HTTP | Rota |
|---|---|
| GET | `GetAll?page&pageSize` |
| GET | `GetById/{id}` |
| POST | `Create` |
| PUT | `Update/{id}` |
| DELETE | `Delete/{id}` |

#### `DashboardController` — `/api/Dashboard`

| Método HTTP | Rota | Query Params |
|---|---|---|
| GET | `/api/Dashboard` | `year`, `month` |

#### `ExpensesController` — `/api/Expenses`

| Método HTTP | Rota |
|---|---|
| GET | `GetAll?page&pageSize&from&to` |
| GET | `GetById/{id}` |
| POST | `Create` |
| PUT | `Update/{id}` |
| DELETE | `Delete/{id}` |

#### `InventoriesController` — `/api/Inventories`

| Método HTTP | Rota |
|---|---|
| GET | `GetInventory` |
| GET | `GetSupplies?page&pageSize` |
| GET | `GetSupplyById/{id}` |
| POST | `CreateSupply` |
| PUT | `UpdateSupply/{id}` |
| DELETE | `DeleteSupply/{id}` |
| GET | `GetFinalProducts?page&pageSize` |
| GET | `GetFinalProductById/{id}` |
| POST | `CreateFinalProduct` |
| PUT | `UpdateFinalProduct/{id}` |
| DELETE | `DeleteFinalProduct/{id}` |
| GET | `final-products/{id}/recipe` |
| POST | `final-products/{id}/recipe` |
| DELETE | `final-products/{id}/recipe/{recipeItemId}` |

#### `MonthlyGoalsController` — `/api/MonthlyGoals`

| Método HTTP | Rota | Observação |
|---|---|---|
| GET | `/api/MonthlyGoals?year&month` | Retorna meta do mês |
| POST | `/api/MonthlyGoals` | Upsert — valor fixo ou % sobre custos |

#### `OrdersController` — `/api/Orders`

| Método HTTP | Rota |
|---|---|
| GET | `GetAll?page&pageSize` |
| GET | `GetById/{id}` |
| POST | `Create` |
| PUT | `Update/{id}` |
| DELETE | `Delete/{id}` |

#### `ReceiptsController` — `/api/Receipts`

| Método HTTP | Rota |
|---|---|
| GET | `GetAll?page&pageSize` |
| GET | `GetById/{id}` |
| POST | `Create` |
| PUT | `Update/{id}` |
| DELETE | `Delete/{id}` |

#### `StockMovementsController` — `/api/StockMovements`

| Método HTTP | Rota |
|---|---|
| GET | `GetAll?page&pageSize` |
| GET | `GetBySupplyId/{supplyId}` |
| GET | `GetByOrderId/{orderId}` |
| POST | `Create` |

---

## Testes

### Localização

```
Tests/
  Application.Tests/
    CreateExpenseCommandHandlerTests.cs
    UpdateOrderCommandHandlerTests.cs
    GetDashboardQueryHandlerTests.cs
    CreateStockMovementCommandHandlerTests.cs
    UpsertMonthlyGoalCommandHandlerTests.cs
```

### Stack de testes

| Lib | Uso |
|---|---|
| `xUnit` | Framework de testes |
| `NSubstitute` | Mocks/stubs |
| `FluentAssertions` | Asserções expressivas |
| `Mapster` | Configuração de mapeamento nos testes |
| `coverlet` | Coleta de cobertura |

### Convenção de teste

- Cada teste de handler isola **exclusivamente** o handler em teste
- Dependências (serviços) são mockadas via `NSubstitute`
- Nomenclatura: `MetodoTestado_Cenario_ResultadoEsperado`
- Seguir padrão **Arrange / Act / Assert**

---

## Regras Críticas para o Agente

1. **Sempre usar TDD** — nenhuma funcionalidade sem teste primeiro
2. **Nunca implementar sem teste** — ciclo: red → green → refactor
3. **Manter Clean Architecture** — dependências sempre no sentido Domain → Application → Infrastructure → WebApi
4. **Usar `ResponseWrapper`** em todo retorno de handler
5. **Validações via FluentValidation** — nunca no handler; usar `IValidateMe` + `IValidator<T>`
6. **Mapeamentos via Mapster** — não usar AutoMapper
7. **IDs como GUID string** — conforme `BaseEntity`
8. **Migrations** — criar via EF Core CLI; jamais alterar tabelas manualmente
9. **Serviços como Scoped** — seguir o padrão do `Program.cs`
10. **Atualizar `docs/MODIFICACOES-BACKEND.md`** ao final de cada sessão de implementação registrando funcionalidades criadas, decisões técnicas e endpoints adicionados

---

## Estado Atual do Projeto

### Funcionalidades implementadas

- [x] Gestão de Clientes (CRUD)
- [x] Gestão de Categorias (CRUD)
- [x] Gestão de Orçamentos (CRUD + conversão para pedido)
- [x] Gestão de Pedidos (CRUD)
- [x] Gestão de Despesas (CRUD + filtro por período + categoria FK)
- [x] Estoque — Insumos, Produtos Finais e Receitas (CRUD + ajuste de estoque)
- [x] Movimentações de Estoque (criação + consultas por insumo/pedido)
- [x] Recibos (CRUD)
- [x] Metas Mensais (Upsert — valor fixo ou % sobre despesas)
- [x] Dashboard (resumo financeiro do mês com meta e lucro)
- [x] Pipeline de validação (FluentValidation via MediatR)
- [x] Swagger habilitado

### Testes existentes

- [x] `CreateExpenseCommandHandlerTests`
- [x] `UpdateOrderCommandHandlerTests`
- [x] `GetDashboardQueryHandlerTests`
- [x] `CreateStockMovementCommandHandlerTests`
- [x] `UpsertMonthlyGoalCommandHandlerTests`

### Pendente / Próximas features

- [ ] Autenticação e autorização
- [ ] Notificações / alertas de estoque baixo
- [ ] Relatórios exportáveis (PDF/Excel)
- [ ] Testes de integração (Infrastructure + SQLite in-memory)
- [ ] Testes de API (Controllers)
- [ ] Ampliação de cobertura de testes unitários

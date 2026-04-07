# Modificações no Backend & Frontend — SweetCandy

Documento técnico que registra **o que foi alterado, o que foi removido, o que foi adicionado** e o **motivo** de cada mudança realizada no projeto. Organizado por sessão/entrega, do mais antigo ao mais recente.

---

## Contexto geral

O backend já seguia Clean Architecture (Domain → Application com MediatR/FluentValidation/Mapster → Infrastructure com EF Core + SQLite → WebApi). Faltavam recursos para finanças, clientes, painel e validações coerentes, além de bugs funcionais na lógica de estoque.

---

## Sessão 1 — Estrutura base, Clientes, Despesas, Estoque, Meta, Painel

### Swagger / Swashbuckle
**O quê:** Pacote `Swashbuckle.AspNetCore` referenciado em `WebApi.csproj`; `AddSwaggerGen()` + `UseSwagger()` + `UseSwaggerUI()` em `Program.cs`.  
**Por quê:** Garantir que a UI do Swagger funcione mesmo sem o pacote pré-instalado no ambiente; facilita testes manuais dos endpoints durante o desenvolvimento.

### CORS
**O quê:** Política `Frontend` em `Program.cs` permitindo origens `localhost:5173` (Vite) e `localhost:3000`.  
**Por quê:** O navegador bloqueia chamadas da SPA para outra origem sem CORS; essas portas são as mais comuns para o frontend em desenvolvimento.

### Feature Clientes (`Customer`)
**Adicionado:** `CustomerService.cs`, `CustomersController.cs`, `DeleteCustomerCommandValidator.cs`.  
**Por quê:** A entidade existia no domínio mas não havia serviço, DI nem API REST. Pedidos e orçamentos dependem de clientes cadastrados.

### Feature Despesas (`Expense`)
**Adicionado:** Campos `Date`, `Category` e `PaymentMethod` na entidade. `ExpenseService.cs`, commands, queries, validators, `ExpensesController`.  
**Por quê:** Completar o modelo de custos do negócio; antes a entidade não tinha API nem Infrastructure.

### Estoque — ajuste manual e histórico (`StockMovement`)
**Adicionado:** `AdjustSupplyStockCommand`, `GetStockMovementsQuery`, rotas `POST inventories/supplies/{id}/adjustments` e `GET inventories/stock-movements`.  
**Por quê:** Permitir registrar entrada/saída de insumos com histórico, necessário para o fluxo de controle de estoque.

### Meta mensal (`MonthlyGoal`)
**Adicionado:** Entidade `MonthlyGoal` (`Year`, `Month`, `TargetAmount`). `MonthlyGoalService`, `UpsertMonthlyGoalCommand`, `GetMonthlyGoalByMonthQuery`, `MonthlyGoalsController`.  
**Por quê:** Suportar a barra de progresso de meta no painel; persistência em banco em vez de valor fixo no frontend.

### Painel agregador (`Dashboard`)
**Adicionado:** `GetDashboardQuery`, `DashboardController` (`GET /api/Dashboard?year=&month=`). Calcula receitas, despesas, lucro e `GoalProgressPercent` por mês.  
**Por quê:** Centralizar em um endpoint os dados da tela inicial; evita que o frontend some várias listas client-side.

---

## Sessão 2 — PaymentMethod em Expense + migration

### `Expense.PaymentMethod`
**Adicionado:** Propriedade `FormaPagamento PaymentMethod` na entidade `Expense` e na DTO `CreateExpenseRequest`.  
**Por quê:** Despesas também têm forma de pagamento; estava presente em `Receipt` mas ausente em `Expense`.

### Migration `AddPaymentMethodToExpense`
**Adicionado:** Colunas `Date`, `Category`, `PaymentMethod` na tabela `Expenses`; nova tabela `MonthlyGoals`.  
**Por quê:** Persistir as novas propriedades no banco SQLite.

---

## Sessão 3 — Validações migradas do frontend para o backend + Meta Inteligente + Dashboard API

### `DashboardResponse` enriquecida
**Arquivo:** `Application/Features/Dashboard/DTOs/DashboardResponse.cs`  
**Adicionado:** `decimal SuggestedGoal`, `decimal EffectiveGoal`, `decimal EffectiveGoalPercent`.  
**Por quê:** O frontend precisava de um único endpoint que retornasse tanto a meta personalizada quanto a meta sugerida automaticamente (gastos × 1,5), sem lógica duplicada client-side.

### `GetDashboardQuery` — cálculo da meta inteligente
**Arquivo:** `Application/Features/Dashboard/Queries/GetDashboardQuery.cs`  
**Adicionado:** `suggestedGoal = expenses × 1.5`, `effectiveGoal = meta_personalizada ?? suggestedGoal`, `effectiveGoalPercent`.  
**Por quê:** A regra "mínimo para não ficar negativo = gastos + 50% de margem" fica centralizada no backend, onde os dados são calculados de forma confiável.

### `UpsertMonthlyGoalRequest` — modo porcentagem
**Arquivo:** `Application/Features/MonthlyGoals/DTOs/UpsertMonthlyGoalRequest.cs`  
**Alterado:** `decimal TargetAmount` → `decimal? TargetAmount` (nullable).  
**Adicionado:** `decimal? PercentageOverCosts` — informa X% sobre os gastos do mês como meta.  
**Por quê:** Permitir dois modos de definição de meta: valor fixo (R$) ou porcentagem sobre os gastos reais do mês, mais intuitivo para confeiteiros.

### `UpsertMonthlyGoalCommand` — lógica de percentual
**Arquivo:** `Application/Features/MonthlyGoals/Commands/UpsertMonthlyGoalCommand.cs`  
**Alterado:** Handler agora injeta `IExpenseService`. Se `PercentageOverCosts` for informado, busca as despesas do mês e calcula `gastos × (1 + pct/100)` como `TargetAmount`.  
**Removido:** Import do `Mapster` (a entidade é montada manualmente para incluir a lógica de cálculo).  
**Por quê:** A regra de negócio "meta = gastos × porcentagem" pertence à camada Application, não ao frontend.

### `UpsertMonthlyGoalCommandValidator` — regra de exclusividade
**Arquivo:** `Application/Features/MonthlyGoals/Validations/UpsertMonthlyGoalCommandValidator.cs`  
**Adicionado:** Regra `.Must(r => r.TargetAmount.HasValue || r.PercentageOverCosts.HasValue)` — pelo menos um dos dois campos deve ser informado. Validação de faixa em `PercentageOverCosts` (0–1000).  
**Removido:** Regra `TargetAmount >= 0` que era sempre obrigatória (agora é opcional e verificada condicionalmente).  
**Por quê:** Garantir que o cliente sempre informe um modo de definição de meta, sem aceitar requests com ambos os campos nulos.

### Validators reforçados — enums e nomes
**Arquivos modificados:**
- `CreateReceiptCommandValidator.cs` — adicionado `.IsInEnum()` em `PaymentMethod`.
- `CreateExpenseCommandValidator.cs` — adicionado `.IsInEnum()` em `PaymentMethod`.
- `UpdateExpenseCommandValidator.cs` — adicionado `Name.NotEmpty().When(name != null)` (antes, atualizar despesa com nome vazio era aceito).
- `CreateStockMovementCommandValidator.cs` — adicionado `.IsInEnum()` em `Type` (MovementType).
- `CreateOrderCommandValidator.cs` — adicionado `.IsInEnum()` em `Status` (StatusOrder).

**Por quê:** Valores de enum inválidos eram aceitos silenciosamente pelo deserializador e salvos no banco. A validação `.IsInEnum()` garante que apenas valores definidos no enum sejam persistidos, evitando dados corrompidos.

### Frontend — 10 guards duplicados removidos
**Arquivo:** `WebApi/wwwroot/app.js`  
**Removido:** 10 chamadas `return toast('...', 'error')` que validavam campos obrigatórios antes de chamar a API (valor > 0, nome obrigatório, data obrigatória, quantidade obrigatória).  
**Por quê:** Essas verificações eram duplicadas — o pipeline FluentValidation do backend (`ValidationPipelineBenaviour`) já retorna os erros e o frontend já os exibia via `toast((r.messages||[]).join(', '), 'error')`. Manter a guarda no frontend criava inconsistência: mensagens diferentes para o mesmo erro dependendo de onde era detectado.

### Frontend — Dashboard consumindo API + UI de Meta
**Arquivo:** `WebApi/wwwroot/app.js`  
**Alterado:** `renderDashboard()` deixou de fazer 4 chamadas separadas (receipts, expenses, orders, customers) e calcular receita/despesa/lucro/meta client-side. Agora faz `GET /api/Dashboard?year=X&month=Y` e usa os campos `revenue`, `expenses`, `profit`, `effectiveGoal`, `effectiveGoalPercent`, `suggestedGoal` retornados pelo endpoint.  
**Removido:** `const metaMensal = 5000` — valor fixo hardcoded que nunca mudava.  
**Adicionado:**
- Botão "Configurar" no card de Meta Mensal.
- Modal `btnConfigMeta()` com toggle Valor fixo (R$) / Porcentagem sobre gastos (%).
- Funções `toggleMetaModo()` e `saveGoal()` que chamam `POST /api/MonthlyGoals/Upsert`.
- Hint automático no card quando a meta não é personalizada.  
**Por quê:** O frontend antes ignorava completamente a funcionalidade de meta mensal persistida no banco. Com essa mudança, a meta definida pela confeiteira é respeitada e a sugestão automática serve como fallback inteligente.

---

## Sessão 4 — Bugs críticos e integridade de dados

### Bug corrigido: `StockMovementService` não atualizava `Supply.Quantity`
**Arquivo:** `Infrastructure/StockMovements/StockMovementService.cs`  
**Problema:** `CreateAsync` registrava a movimentação no banco mas **nunca alterava o `Quantity` do insumo associado**. O saldo do estoque era estático — só mudava via edição manual do insumo.  
**Correção:** Antes de persistir a movimentação, o método agora:
1. Busca o `Supply` pelo `SupplyId`.
2. Se não encontrado, retorna erro descritivo (em vez de ignorar silenciosamente).
3. Se `Type == Saida` e o estoque for insuficiente, retorna `"Estoque insuficiente. Disponível: X, solicitado: Y."` — impedindo estoque negativo.
4. Caso contrário, soma (Entrada) ou subtrai (Saida) a quantidade e persiste.  
**Por quê:** Regra de negócio fundamental — movimentar estoque sem atualizar o saldo torna o sistema de estoque inutilizável.

### `Expense.CategoryId` — FK real para `Category`
**Arquivos modificados:**
- `Domain/Entities/Expense.cs`
- `Application/Features/Expenses/DTOs/ExpenseResponse.cs`
- `Application/Features/Expenses/DTOs/CreateExpenseRequest.cs`
- `Application/Features/Expenses/DTOs/UpdateExpenseRequest.cs`
- `Infrastructure/Expenses/ExpenseService.cs`
- `Infrastructure/Persistence/AppDbContext.cs`

**O que mudou:**
| Antes | Depois |
|---|---|
| `string? Category` (texto livre) | `string? CategoryName` (texto livre, legado) |
| — | `string? CategoryId` (FK para tabela Categories) |
| — | `Category? Category` (navigation property) |

**Por quê:** A entidade `Category` já existia com CRUD completo. `Expense.Category` era um campo texto solto — renomear uma categoria no sistema não afetava as despesas já cadastradas (inconsistência de dados). Com `CategoryId` como chave estrangeira, o relacionamento é real e o banco pode impor integridade referencial. O campo `CategoryName` é mantido para compatibilidade com dados antigos e para o fluxo "quick add" do front onde o usuário digita texto livre.

### Migration `AddExpenseCategoryFK`
**Adicionado:** `migrationBuilder.RenameColumn("Category", "Expenses", "CategoryName")` + coluna `CategoryId TEXT` com índice e FK para `Categories.Id`.  
**Por quê:** Persistir o renomeamento e o novo relacionamento no banco SQLite sem perder dados existentes.

### FK `Expense → Category` no `AppDbContext`
**Arquivo:** `Infrastructure/Persistence/AppDbContext.cs`  
**Adicionado:**
```csharp
modelBuilder.Entity<Expense>()
  .HasOne(e => e.Category)
  .WithMany()
  .HasForeignKey(e => e.CategoryId)
  .IsRequired(false)
  .OnDelete(DeleteBehavior.SetNull);
```
**Por quê:** Configurar explicitamente o comportamento: ao deletar uma categoria, as despesas que a referenciavam têm `CategoryId` zerado (SetNull) em vez de serem excluídas em cascata — o que faria sentido para dados financeiros históricos.

### Frontend — dropdown de categorias na edição de despesas
**Arquivo:** `WebApi/wwwroot/app.js`  
**Alterado:**
- `editDespesa(e)` → agora é `async`, carrega `GET /api/Categories/GetAll` antes de abrir o modal.
- `formDespesa(e, catOpts)` → aceita segundo parâmetro com as `<option>` pré-geradas das categorias, renderizando um `<select>` em vez de texto livre quando disponível.
- `createDespesa()` e `saveDespesa()` → enviam `categoryId` (em vez de texto livre) para a API.
- `saveTransacao()` → campo `category:` renomeado para `categoryName:` para refletir o campo da DTO.  
**Por quê:** Com a FK real no backend, o frontend precisa enviar o `id` da categoria. O dropdown garante que o usuário selecione uma categoria existente em vez de digitá-la livremente (evitando inconsistências). O quick-add (`buildTransacaoForm`) ainda aceita texto livre mapeado para `categoryName` para fluxo rápido.

---

## Itens já implementados (contrariamente ao que parecia pendente)

### Pedido Concluído → Receita automática (item 4 da análise)
**Arquivo:** `Application/Features/Orders/Commands/UpdateOrderCommand.cs`  
**Status:** Já estava implementado. Quando `order.Status` muda para `Concluída`, um `Receipt` é criado automaticamente com `Amount = order.TotalValue`, `OrderId = order.Id`, `CustomerId = order.CustomerId`.  
**Adicionalmente:** Quando status muda para `Confirmada`, movimentações de estoque de Saída são criadas automaticamente para cada item do pedido.

---

## Itens deliberadamente não implementados

### Paginação real no banco (item 5)
**Situação:** Os `GetAllAsync()` ainda retornam `ToListAsync()` (sem `.Skip().Take()`).  
**Por quê não implementado:** Impacto baixo a curto prazo para o volume esperado de uma confeitaria. Exigiria alterar todas as interfaces de serviço, queries handlers e o frontend simultaneamente — alto esforço com benefício não imediato.

### Substituição de `string` de retorno por `Result<T>` (item 6)
**Situação:** Services ainda retornam `string.Empty` (sucesso) ou mensagem de erro.  
**Por quê não implementado:** Refactor de qualidade sem impacto funcional imediato. Os handlers já convertem corretamente para `ResponseWrapper`. Prioridade baixa frente a bugs funcionais.

---

## Estado final do build

```
Build succeeded.
0 Warning(s)
0 Error(s)
```

Todas as alterações acima compilam sem erros ou avisos.

---

## Sessão 5 — Correção de bug introduzido + validação de FK + UX de categoria

### Bug corrigido: `CreateStockMovementCommandHandler` ignorava erros do Service
**Arquivo:** `Application/Features/StockMovements/Commands/CreateStockMovementCommand.cs`  
**Problema:** Após a correção do `StockMovementService` (Sessão 4), o `CreateAsync` passou a retornar mensagens de erro em vez de um ID quando o estoque é insuficiente ou o insumo não existe. O Handler, no entanto, **assumia que o retorno era sempre um ID** e respondia com sucesso incondicional — ou seja, o front recebia `"Movimentação criada. Id: Estoque insuficiente..."` em vez de uma mensagem de erro.  
**Correção:** O Handler agora verifica o retorno com `Guid.TryParse(result, out _)`. Se não for um GUID válido, trata como mensagem de erro e retorna `ResponseWrapper.FailAsync(result)`.  
**Por quê:** A convenção `string.Empty = sucesso / mensagem = erro` exige que o chamador verifique o retorno. O Handler não fazia isso, criando uma resposta enganosa para o cliente.

### Bug corrigido: `UpdateOrderCommand` ignorava falhas na baixa automática de estoque
**Arquivo:** `Application/Features/Orders/Commands/UpdateOrderCommand.cs`  
**Problema:** Quando um pedido era confirmado, a baixa automática de estoque era disparada para cada item. Se algum insumo não tivesse estoque suficiente, o erro era silenciosamente descartado — o pedido era confirmado, mas o estoque não era baixado.  
**Correção:** Os erros de cada `_stockMovementService.CreateAsync(movement)` são coletados numa lista. Se houver algum, o Handler retorna `FailAsync` com a mensagem consolidada, impedindo a confirmação silenciosa com estoque inconsistente.  
**Por quê:** Confirmar um pedido sem baixar o estoque cria divergência entre o status de produção e o saldo real de insumos — um problema operacional grave para a confeiteira.

### `CreateExpenseCommand` — validação do `CategoryId`
**Arquivo:** `Application/Features/Expenses/Commands/CreateExpenseCommand.cs`  
**Problema:** Se o frontend enviava um `categoryId` inválido (GUID inexistente no banco), o Mapster mapeava o campo direto para a entidade e o EF Core lançava uma exceção de violação de FK constraint — retornando um erro 500 não tratado em vez de uma mensagem amigável.  
**Adicionado:** Injeção de `ICategoryService` no Handler. Antes de persistir, se `CategoryId` for informado, verifica a existência com `GetByIdAsync`. Retorna `FailAsync("Categoria não encontrada.")` se não existir.  
**Por quê:** Validação de existência de FK é responsabilidade da camada Application (regra de negócio), não do banco. Essa verificação já era feita em `CreateFinalProductCommand` para a mesma `Category` — padroniza o comportamento.

### Frontend — categoria exibida na listagem de despesas
**Arquivo:** `WebApi/wwwroot/app.js`  
**Problema:** A categoria foi implementada como FK e passa a ser cadastrada pelos usuários, mas a listagem de despesas não a exibia — o usuário definia a categoria no modal mas ela era invisível na tela principal.  
**Alterado:** O item de despesa na lista agora mostra `e.categoryName` com um ícone de tag (`fa-tag`), quando presente. O campo é opcional — se a despesa não tiver categoria, nada é exibido.  
**Por quê:** Informação cadastrada mas invisível é desperdício de UX. A categoria é o principal filtro que a confeiteira usa para entender onde está gastando (insumos, aluguel, energia etc.).

---

## Estado final do build

```
Build succeeded.
0 Warning(s)
0 Error(s)
```

Todas as alterações acima compilam sem erros ou avisos.

---

*Última atualização: 07/04/2026*


# SweetCandy API — Documentação para Frontend

> **Base URL:** `http://localhost:{porta}/api`
>
> **Swagger UI:** `http://localhost:{porta}/swagger`

---

## Envelope Padrão (Response Wrapper)

Todas as respostas seguem este formato:

**Sem dados** (Create / Update / Delete):
```json
{
  "messages": ["Operação realizada com sucesso"],
  "isSuccessful": true
}
```

**Com dados** (Queries):
```json
{
  "messages": [],
  "isSuccessful": true,
  "data": { }
}
```

**Erro de validação:**
```json
{
  "messages": [
    "Name é obrigatório",
    "Quantity deve ser maior que zero"
  ],
  "isSuccessful": false
}
```

---

## Paginação

Endpoints `GetAll` aceitam query parameters:

| Parâmetro  | Tipo  | Padrão | Descrição                    |
|------------|-------|--------|------------------------------|
| `page`     | int   | 1      | Número da página             |
| `pageSize` | int   | 20     | Quantidade de itens por página |

Exemplo: `GET api/Customers/GetAll?page=2&pageSize=10`

---

## Enums (enviar como inteiro)

### StatusOrder
| Valor | Int | Descrição       |
|-------|-----|-----------------|
| Pendente   | 0 | Pedido criado, aguardando confirmação |
| Confirmada | 1 | Pedido confirmado (gera saída de estoque) |
| Cancelada  | 2 | Pedido cancelado |
| Concluida  | 3 | Pedido concluído (gera recibo automático) |

### FormaPagamento
| Valor    | Int |
|----------|-----|
| Dinheiro | 0   |
| Debito   | 1   |
| Credito  | 2   |
| Pix      | 3   |

### Unidade
| Valor   | Int |
|---------|-----|
| Un      | 0   |
| Kg      | 1   |
| G       | 2   |
| L       | 3   |
| Ml      | 4   |
| Mg      | 5   |
| Caixa   | 6   |
| Pacote  | 7   |

### MovementType
| Valor   | Int |
|---------|-----|
| Saida   | 0   |
| Entrada | 1   |

---

## Observações Gerais

- **Partial Update:** Nos endpoints de Update, campos enviados como `null` **não** sobrescrevem o valor existente. Envie somente os campos que deseja alterar.
- **IDs:** Gerados automaticamente pelo backend (GUID). Não enviar no Create.
- **Datas:** Formato ISO 8601 — `"2026-04-15T14:30:00"`

---

# 1. Customers (Clientes)

**Prefixo:** `api/Customers`

## GET `/GetAll?page=1&pageSize=20`

**Response:**
```json
{
  "messages": [],
  "isSuccessful": true,
  "data": [
    {
      "id": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
      "name": "Maria Silva",
      "email": "maria@email.com",
      "phone": "(11) 99999-0000",
      "address": "Rua das Flores, 123 - São Paulo/SP",
      "birthDate": "1990-05-15T00:00:00",
      "createdAt": "2026-03-01T10:00:00",
      "updatedAt": "2026-03-01T10:00:00"
    }
  ]
}
```

## GET `/GetById/{id}`

**Response:**
```json
{
  "messages": [],
  "isSuccessful": true,
  "data": {
    "id": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
    "name": "Maria Silva",
    "email": "maria@email.com",
    "phone": "(11) 99999-0000",
    "address": "Rua das Flores, 123 - São Paulo/SP",
    "birthDate": "1990-05-15T00:00:00",
    "createdAt": "2026-03-01T10:00:00",
    "updatedAt": "2026-03-01T10:00:00"
  }
}
```

## POST `/Create`

**Request Body:**
```json
{
  "name": "Maria Silva",
  "email": "maria@email.com",
  "phone": "(11) 99999-0000",
  "address": "Rua das Flores, 123 - São Paulo/SP",
  "birthDate": "1990-05-15T00:00:00"
}
```

**Response:**
```json
{
  "messages": ["Customer criado com sucesso"],
  "isSuccessful": true
}
```

## PUT `/Update/{id}`

**Request Body (partial — envie somente o que mudar):**
```json
{
  "phone": "(11) 98888-1111",
  "address": "Av. Paulista, 1000 - São Paulo/SP"
}
```

**Response:**
```json
{
  "messages": ["Customer atualizado com sucesso"],
  "isSuccessful": true
}
```

## DELETE `/Delete/{id}`

**Response:**
```json
{
  "messages": ["Customer removido com sucesso"],
  "isSuccessful": true
}
```

---

# 2. Categories (Categorias)

**Prefixo:** `api/Categories`

## GET `/GetAll?page=1&pageSize=20`

**Response:**
```json
{
  "messages": [],
  "isSuccessful": true,
  "data": [
    {
      "id": "b2c3d4e5-f6a7-8901-bcde-f12345678901",
      "name": "Bolos",
      "description": "Bolos decorados e temáticos"
    },
    {
      "id": "c3d4e5f6-a7b8-9012-cdef-123456789012",
      "name": "Docinhos",
      "description": "Brigadeiros, beijinhos e trufas"
    }
  ]
}
```

## GET `/GetById/{id}`

**Response:**
```json
{
  "messages": [],
  "isSuccessful": true,
  "data": {
    "id": "b2c3d4e5-f6a7-8901-bcde-f12345678901",
    "name": "Bolos",
    "description": "Bolos decorados e temáticos"
  }
}
```

## POST `/Create`

**Request Body:**
```json
{
  "name": "Bolos",
  "description": "Bolos decorados e temáticos"
}
```

**Response:**
```json
{
  "messages": ["Categoria criada com sucesso"],
  "isSuccessful": true
}
```

## PUT `/Update/{id}`

**Request Body:**
```json
{
  "description": "Bolos artesanais decorados"
}
```

**Response:**
```json
{
  "messages": ["Categoria atualizada com sucesso"],
  "isSuccessful": true
}
```

## DELETE `/Delete/{id}`

**Response:**
```json
{
  "messages": ["Categoria removida com sucesso"],
  "isSuccessful": true
}
```

---

# 3. Inventories — Supplies (Insumos)

**Prefixo:** `api/Inventories`

## GET `/GetInventory`

**Response:**
```json
{
  "messages": [],
  "isSuccessful": true,
  "data": {
    "id": "d4e5f6a7-b8c9-0123-defa-234567890123",
    "totalInvested": 1250.75,
    "supplies": [
      {
        "id": "e5f6a7b8-c9d0-1234-efab-345678901234",
        "name": "Farinha de Trigo",
        "quantity": 10.0,
        "price": 8.50,
        "unit": 1,
        "totalPrice": 85.00,
        "inventoryId": "d4e5f6a7-b8c9-0123-defa-234567890123"
      },
      {
        "id": "f6a7b8c9-d0e1-2345-fabc-456789012345",
        "name": "Açúcar",
        "quantity": 5.0,
        "price": 6.00,
        "unit": 1,
        "totalPrice": 30.00,
        "inventoryId": "d4e5f6a7-b8c9-0123-defa-234567890123"
      }
    ]
  }
}
```

> **unit:** `0=Un, 1=Kg, 2=G, 3=L, 4=Ml, 5=Mg, 6=Caixa, 7=Pacote`

## GET `/GetSupplies?page=1&pageSize=20`

**Response:**
```json
{
  "messages": [],
  "isSuccessful": true,
  "data": [
    {
      "id": "e5f6a7b8-c9d0-1234-efab-345678901234",
      "name": "Farinha de Trigo",
      "quantity": 10.0,
      "price": 8.50,
      "unit": 1,
      "totalPrice": 85.00,
      "inventoryId": "d4e5f6a7-b8c9-0123-defa-234567890123"
    }
  ]
}
```

## GET `/GetSupplyById/{id}`

**Response:**
```json
{
  "messages": [],
  "isSuccessful": true,
  "data": {
    "id": "e5f6a7b8-c9d0-1234-efab-345678901234",
    "name": "Farinha de Trigo",
    "quantity": 10.0,
    "price": 8.50,
    "unit": 1,
    "totalPrice": 85.00,
    "inventoryId": "d4e5f6a7-b8c9-0123-defa-234567890123"
  }
}
```

## POST `/CreateSupply`

**Request Body:**
```json
{
  "name": "Farinha de Trigo",
  "quantity": 10.0,
  "price": 8.50,
  "unit": 1,
  "inventoryId": "d4e5f6a7-b8c9-0123-defa-234567890123"
}
```

**Response:**
```json
{
  "messages": ["Insumo criado com sucesso"],
  "isSuccessful": true
}
```

## PUT `/UpdateSupply/{id}`

**Request Body:**
```json
{
  "quantity": 15.0,
  "price": 9.00
}
```

**Response:**
```json
{
  "messages": ["Insumo atualizado com sucesso"],
  "isSuccessful": true
}
```

## DELETE `/DeleteSupply/{id}`

**Response:**
```json
{
  "messages": ["Insumo removido com sucesso"],
  "isSuccessful": true
}
```

---

# 4. Inventories — Final Products (Produtos Finais)

**Prefixo:** `api/Inventories`

## GET `/GetFinalProducts?page=1&pageSize=20`

**Response:**
```json
{
  "messages": [],
  "isSuccessful": true,
  "data": [
    {
      "id": "a7b8c9d0-e1f2-3456-abcd-567890123456",
      "name": "Bolo de Chocolate",
      "description": "Bolo de chocolate com cobertura de ganache",
      "categoryId": "b2c3d4e5-f6a7-8901-bcde-f12345678901",
      "costPrice": 45.00,
      "unitPrice": 120.00,
      "quantityAvailable": 3.0
    },
    {
      "id": "b8c9d0e1-f2a3-4567-bcde-678901234567",
      "name": "Brigadeiro Gourmet",
      "description": "Brigadeiro com chocolate belga",
      "categoryId": "c3d4e5f6-a7b8-9012-cdef-123456789012",
      "costPrice": 1.50,
      "unitPrice": 5.00,
      "quantityAvailable": 100.0
    }
  ]
}
```

## GET `/GetFinalProductById/{id}`

**Response:**
```json
{
  "messages": [],
  "isSuccessful": true,
  "data": {
    "id": "a7b8c9d0-e1f2-3456-abcd-567890123456",
    "name": "Bolo de Chocolate",
    "description": "Bolo de chocolate com cobertura de ganache",
    "categoryId": "b2c3d4e5-f6a7-8901-bcde-f12345678901",
    "costPrice": 45.00,
    "unitPrice": 120.00,
    "quantityAvailable": 3.0
  }
}
```

## POST `/CreateFinalProduct`

**Request Body:**
```json
{
  "name": "Bolo de Chocolate",
  "description": "Bolo de chocolate com cobertura de ganache",
  "categoryId": "b2c3d4e5-f6a7-8901-bcde-f12345678901",
  "costPrice": 45.00,
  "unitPrice": 120.00,
  "quantityAvailable": 3.0
}
```

**Response:**
```json
{
  "messages": ["Produto final criado com sucesso"],
  "isSuccessful": true
}
```

## PUT `/UpdateFinalProduct/{id}`

**Request Body:**
```json
{
  "unitPrice": 130.00,
  "quantityAvailable": 5.0
}
```

**Response:**
```json
{
  "messages": ["Produto final atualizado com sucesso"],
  "isSuccessful": true
}
```

## DELETE `/DeleteFinalProduct/{id}`

**Response:**
```json
{
  "messages": ["Produto final removido com sucesso"],
  "isSuccessful": true
}
```

---

# 5. Budgets (Orçamentos)

**Prefixo:** `api/Budgets`

## GET `/GetAll?page=1&pageSize=20`

**Response:**
```json
{
  "messages": [],
  "isSuccessful": true,
  "data": [
    {
      "id": "c9d0e1f2-a3b4-5678-cdef-789012345678",
      "clientName": "Maria Silva",
      "eventDate": "2026-05-20T00:00:00",
      "finalProductName": "Bolo de Casamento",
      "finalProductDescription": "Bolo 3 andares com flores",
      "finalProductQuantity": 1.0,
      "finalUnitPrice": 350.00,
      "finalTotalValue": 350.00,
      "items": [
        {
          "id": "d0e1f2a3-b4c5-6789-defa-890123456789",
          "finalProductId": "a7b8c9d0-e1f2-3456-abcd-567890123456",
          "finalProductName": "Bolo de Chocolate",
          "quantity": 1.0,
          "unitPrice": 120.00,
          "totalPrice": 120.00
        },
        {
          "id": "e1f2a3b4-c5d6-7890-efab-901234567890",
          "finalProductId": "b8c9d0e1-f2a3-4567-bcde-678901234567",
          "finalProductName": "Brigadeiro Gourmet",
          "quantity": 100.0,
          "unitPrice": 5.00,
          "totalPrice": 500.00
        }
      ]
    }
  ]
}
```

## GET `/GetById/{id}`

**Response:**
```json
{
  "messages": [],
  "isSuccessful": true,
  "data": {
    "id": "c9d0e1f2-a3b4-5678-cdef-789012345678",
    "clientName": "Maria Silva",
    "eventDate": "2026-05-20T00:00:00",
    "finalProductName": "Bolo de Casamento",
    "finalProductDescription": "Bolo 3 andares com flores",
    "finalProductQuantity": 1.0,
    "finalUnitPrice": 350.00,
    "finalTotalValue": 350.00,
    "items": [
      {
        "id": "d0e1f2a3-b4c5-6789-defa-890123456789",
        "finalProductId": "a7b8c9d0-e1f2-3456-abcd-567890123456",
        "finalProductName": "Bolo de Chocolate",
        "quantity": 1.0,
        "unitPrice": 120.00,
        "totalPrice": 120.00
      }
    ]
  }
}
```

## POST `/Create`

**Request Body:**
```json
{
  "clientName": "Maria Silva",
  "customerId": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
  "eventDate": "2026-05-20T00:00:00",
  "finalProductName": "Bolo de Casamento",
  "finalProductDescription": "Bolo 3 andares com flores",
  "finalProductQuantity": 1.0,
  "finalUnitPrice": 350.00,
  "finalTotalValue": 350.00,
  "items": [
    {
      "finalProductId": "a7b8c9d0-e1f2-3456-abcd-567890123456",
      "finalProductName": "Bolo de Chocolate",
      "quantity": 1.0,
      "unitPrice": 120.00
    },
    {
      "finalProductId": "b8c9d0e1-f2a3-4567-bcde-678901234567",
      "finalProductName": "Brigadeiro Gourmet",
      "quantity": 100.0,
      "unitPrice": 5.00
    }
  ]
}
```

**Response:**
```json
{
  "messages": ["Orçamento criado com sucesso"],
  "isSuccessful": true
}
```

## PUT `/Update/{id}`

**Request Body:**
```json
{
  "eventDate": "2026-06-10T00:00:00",
  "finalUnitPrice": 400.00,
  "finalTotalValue": 400.00,
  "items": [
    {
      "finalProductId": "a7b8c9d0-e1f2-3456-abcd-567890123456",
      "finalProductName": "Bolo de Chocolate",
      "quantity": 2.0,
      "unitPrice": 120.00
    }
  ]
}
```

**Response:**
```json
{
  "messages": ["Orçamento atualizado com sucesso"],
  "isSuccessful": true
}
```

## DELETE `/Delete/{id}`

**Response:**
```json
{
  "messages": ["Orçamento removido com sucesso"],
  "isSuccessful": true
}
```

## POST `/ConvertToOrder/{id}`

> Converte um orçamento existente em um pedido (Order). Os `BudgetItems` são mapeados para `OrderItems`.

**Request Body:** Nenhum

**Response:**
```json
{
  "messages": ["Orçamento convertido em pedido com sucesso"],
  "isSuccessful": true
}
```

---

# 6. Orders (Pedidos)

**Prefixo:** `api/Orders`

## GET `/GetAll?page=1&pageSize=20`

**Response:**
```json
{
  "messages": [],
  "isSuccessful": true,
  "data": [
    {
      "id": "f2a3b4c5-d6e7-8901-fabc-012345678901",
      "name": "Pedido Casamento Maria",
      "customerId": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
      "eventDate": "2026-05-20T00:00:00",
      "status": 0,
      "sinal": 150.00,
      "totalValue": 970.00,
      "items": [
        {
          "id": "a3b4c5d6-e7f8-9012-abcd-123456789012",
          "finalProductId": "a7b8c9d0-e1f2-3456-abcd-567890123456",
          "finalProductName": "Bolo de Chocolate",
          "quantity": 1.0,
          "unitPrice": 120.00,
          "totalPrice": 120.00
        },
        {
          "id": "b4c5d6e7-f8a9-0123-bcde-234567890123",
          "finalProductId": "b8c9d0e1-f2a3-4567-bcde-678901234567",
          "finalProductName": "Brigadeiro Gourmet",
          "quantity": 100.0,
          "unitPrice": 5.00,
          "totalPrice": 500.00
        }
      ]
    }
  ]
}
```

> **status:** `0=Pendente, 1=Confirmada, 2=Cancelada, 3=Concluida`

## GET `/GetById/{id}`

**Response:**
```json
{
  "messages": [],
  "isSuccessful": true,
  "data": {
    "id": "f2a3b4c5-d6e7-8901-fabc-012345678901",
    "name": "Pedido Casamento Maria",
    "customerId": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
    "eventDate": "2026-05-20T00:00:00",
    "status": 0,
    "sinal": 150.00,
    "totalValue": 970.00,
    "items": [
      {
        "id": "a3b4c5d6-e7f8-9012-abcd-123456789012",
        "finalProductId": "a7b8c9d0-e1f2-3456-abcd-567890123456",
        "finalProductName": "Bolo de Chocolate",
        "quantity": 1.0,
        "unitPrice": 120.00,
        "totalPrice": 120.00
      }
    ]
  }
}
```

## POST `/Create`

**Request Body:**
```json
{
  "name": "Pedido Casamento Maria",
  "customerId": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
  "eventDate": "2026-05-20T00:00:00",
  "status": 0,
  "sinal": 150.00,
  "totalValue": 970.00,
  "items": [
    {
      "finalProductId": "a7b8c9d0-e1f2-3456-abcd-567890123456",
      "finalProductName": "Bolo de Chocolate",
      "quantity": 1.0,
      "unitPrice": 120.00
    },
    {
      "finalProductId": "b8c9d0e1-f2a3-4567-bcde-678901234567",
      "finalProductName": "Brigadeiro Gourmet",
      "quantity": 100.0,
      "unitPrice": 5.00
    }
  ]
}
```

**Response:**
```json
{
  "messages": ["Pedido criado com sucesso"],
  "isSuccessful": true
}
```

## PUT `/Update/{id}`

**Request Body:**
```json
{
  "status": 1,
  "sinal": 200.00
}
```

> **Efeitos colaterais por status:**
> - Mudança para `1` (Confirmada) → Cria automaticamente `StockMovement` (saída) para cada item do pedido
> - Mudança para `3` (Concluida) → Cria automaticamente um `Receipt` (recibo) para o pedido

**Response:**
```json
{
  "messages": ["Pedido atualizado com sucesso"],
  "isSuccessful": true
}
```

## DELETE `/Delete/{id}`

**Response:**
```json
{
  "messages": ["Pedido removido com sucesso"],
  "isSuccessful": true
}
```

---

# 7. Receipts (Recibos)

**Prefixo:** `api/Receipts`

## GET `/GetAll?page=1&pageSize=20`

**Response:**
```json
{
  "messages": [],
  "isSuccessful": true,
  "data": [
    {
      "id": "c5d6e7f8-a9b0-1234-cdef-345678901234",
      "date": "2026-05-20T15:30:00",
      "finalProductName": "Bolo de Chocolate",
      "amount": 970.00,
      "description": "Recibo do pedido Casamento Maria",
      "paymentMethod": 3,
      "orderId": "f2a3b4c5-d6e7-8901-fabc-012345678901",
      "customerId": "a1b2c3d4-e5f6-7890-abcd-ef1234567890"
    }
  ]
}
```

> **paymentMethod:** `0=Dinheiro, 1=Debito, 2=Credito, 3=Pix`

## GET `/GetById/{id}`

**Response:**
```json
{
  "messages": [],
  "isSuccessful": true,
  "data": {
    "id": "c5d6e7f8-a9b0-1234-cdef-345678901234",
    "date": "2026-05-20T15:30:00",
    "finalProductName": "Bolo de Chocolate",
    "amount": 970.00,
    "description": "Recibo do pedido Casamento Maria",
    "paymentMethod": 3,
    "orderId": "f2a3b4c5-d6e7-8901-fabc-012345678901",
    "customerId": "a1b2c3d4-e5f6-7890-abcd-ef1234567890"
  }
}
```

## POST `/Create`

**Request Body:**
```json
{
  "date": "2026-05-20T15:30:00",
  "finalProductName": "Bolo de Chocolate",
  "amount": 970.00,
  "description": "Pagamento integral do pedido",
  "paymentMethod": 3,
  "orderId": "f2a3b4c5-d6e7-8901-fabc-012345678901",
  "customerId": "a1b2c3d4-e5f6-7890-abcd-ef1234567890"
}
```

**Response:**
```json
{
  "messages": ["Recibo criado com sucesso"],
  "isSuccessful": true
}
```

## PUT `/Update/{id}`

**Request Body:**
```json
{
  "amount": 1000.00,
  "paymentMethod": 2
}
```

**Response:**
```json
{
  "messages": ["Recibo atualizado com sucesso"],
  "isSuccessful": true
}
```

## DELETE `/Delete/{id}`

**Response:**
```json
{
  "messages": ["Recibo removido com sucesso"],
  "isSuccessful": true
}
```

---

# 8. Expenses (Despesas)

**Prefixo:** `api/Expenses`

## GET `/GetAll?page=1&pageSize=20`

**Response:**
```json
{
  "messages": [],
  "isSuccessful": true,
  "data": [
    {
      "id": "d6e7f8a9-b0c1-2345-defa-456789012345",
      "name": "Conta de Luz",
      "value": 250.00,
      "paid": true,
      "totalExpense": 250.00
    },
    {
      "id": "e7f8a9b0-c1d2-3456-efab-567890123456",
      "name": "Aluguel",
      "value": 1500.00,
      "paid": false,
      "totalExpense": 1500.00
    }
  ]
}
```

## GET `/GetById/{id}`

**Response:**
```json
{
  "messages": [],
  "isSuccessful": true,
  "data": {
    "id": "d6e7f8a9-b0c1-2345-defa-456789012345",
    "name": "Conta de Luz",
    "value": 250.00,
    "paid": true,
    "totalExpense": 250.00
  }
}
```

## POST `/Create`

**Request Body:**
```json
{
  "name": "Conta de Luz",
  "value": 250.00,
  "paid": false
}
```

**Response:**
```json
{
  "messages": ["Despesa criada com sucesso"],
  "isSuccessful": true
}
```

## PUT `/Update/{id}`

**Request Body:**
```json
{
  "paid": true
}
```

**Response:**
```json
{
  "messages": ["Despesa atualizada com sucesso"],
  "isSuccessful": true
}
```

## DELETE `/Delete/{id}`

**Response:**
```json
{
  "messages": ["Despesa removida com sucesso"],
  "isSuccessful": true
}
```

---

# 9. Stock Movements (Movimentações de Estoque)

**Prefixo:** `api/StockMovements`

> **Atenção:** Movimentações são **imutáveis** — não possuem Update nem Delete. Funcionam como um log de auditoria.

## GET `/GetAll?page=1&pageSize=20`

**Response:**
```json
{
  "messages": [],
  "isSuccessful": true,
  "data": [
    {
      "id": "f8a9b0c1-d2e3-4567-fabc-678901234567",
      "date": "2026-04-01T09:00:00",
      "supplyId": "e5f6a7b8-c9d0-1234-efab-345678901234",
      "quantity": 10.0,
      "type": 1,
      "orderId": null,
      "notes": "Compra de farinha de trigo"
    },
    {
      "id": "a9b0c1d2-e3f4-5678-abcd-789012345678",
      "date": "2026-04-02T14:00:00",
      "supplyId": "e5f6a7b8-c9d0-1234-efab-345678901234",
      "quantity": 2.0,
      "type": 0,
      "orderId": "f2a3b4c5-d6e7-8901-fabc-012345678901",
      "notes": "Saída para pedido Casamento Maria"
    }
  ]
}
```

> **type:** `0=Saida, 1=Entrada`

## GET `/GetBySupplyId/{supplyId}`

> Retorna todas as movimentações de um insumo específico.

**Response:**
```json
{
  "messages": [],
  "isSuccessful": true,
  "data": [
    {
      "id": "f8a9b0c1-d2e3-4567-fabc-678901234567",
      "date": "2026-04-01T09:00:00",
      "supplyId": "e5f6a7b8-c9d0-1234-efab-345678901234",
      "quantity": 10.0,
      "type": 1,
      "orderId": null,
      "notes": "Compra de farinha de trigo"
    }
  ]
}
```

## GET `/GetByOrderId/{orderId}`

> Retorna todas as movimentações vinculadas a um pedido.

**Response:**
```json
{
  "messages": [],
  "isSuccessful": true,
  "data": [
    {
      "id": "a9b0c1d2-e3f4-5678-abcd-789012345678",
      "date": "2026-04-02T14:00:00",
      "supplyId": "e5f6a7b8-c9d0-1234-efab-345678901234",
      "quantity": 2.0,
      "type": 0,
      "orderId": "f2a3b4c5-d6e7-8901-fabc-012345678901",
      "notes": "Saída automática - pedido confirmado"
    }
  ]
}
```

## POST `/Create`

**Request Body:**
```json
{
  "supplyId": "e5f6a7b8-c9d0-1234-efab-345678901234",
  "quantity": 10.0,
  "type": 1,
  "orderId": null,
  "notes": "Compra de farinha de trigo"
}
```

**Response:**
```json
{
  "messages": ["Movimentação de estoque criada com sucesso"],
  "isSuccessful": true
}
```

---

# Resumo de Endpoints

| Módulo | Método | Rota | Body | Descrição |
|--------|--------|------|------|-----------|
| **Customers** | `GET` | `/api/Customers/GetAll` | — | Listar clientes |
| | `GET` | `/api/Customers/GetById/{id}` | — | Buscar cliente |
| | `POST` | `/api/Customers/Create` | ✅ | Criar cliente |
| | `PUT` | `/api/Customers/Update/{id}` | ✅ | Atualizar cliente |
| | `DELETE` | `/api/Customers/Delete/{id}` | — | Remover cliente |
| **Categories** | `GET` | `/api/Categories/GetAll` | — | Listar categorias |
| | `GET` | `/api/Categories/GetById/{id}` | — | Buscar categoria |
| | `POST` | `/api/Categories/Create` | ✅ | Criar categoria |
| | `PUT` | `/api/Categories/Update/{id}` | ✅ | Atualizar categoria |
| | `DELETE` | `/api/Categories/Delete/{id}` | — | Remover categoria |
| **Inventories** | `GET` | `/api/Inventories/GetInventory` | — | Inventário completo |
| | `GET` | `/api/Inventories/GetSupplies` | — | Listar insumos |
| | `GET` | `/api/Inventories/GetSupplyById/{id}` | — | Buscar insumo |
| | `POST` | `/api/Inventories/CreateSupply` | ✅ | Criar insumo |
| | `PUT` | `/api/Inventories/UpdateSupply/{id}` | ✅ | Atualizar insumo |
| | `DELETE` | `/api/Inventories/DeleteSupply/{id}` | — | Remover insumo |
| | `GET` | `/api/Inventories/GetFinalProducts` | — | Listar produtos finais |
| | `GET` | `/api/Inventories/GetFinalProductById/{id}` | — | Buscar produto final |
| | `POST` | `/api/Inventories/CreateFinalProduct` | ✅ | Criar produto final |
| | `PUT` | `/api/Inventories/UpdateFinalProduct/{id}` | ✅ | Atualizar produto final |
| | `DELETE` | `/api/Inventories/DeleteFinalProduct/{id}` | — | Remover produto final |
| **Budgets** | `GET` | `/api/Budgets/GetAll` | — | Listar orçamentos |
| | `GET` | `/api/Budgets/GetById/{id}` | — | Buscar orçamento |
| | `POST` | `/api/Budgets/Create` | ✅ | Criar orçamento |
| | `PUT` | `/api/Budgets/Update/{id}` | ✅ | Atualizar orçamento |
| | `DELETE` | `/api/Budgets/Delete/{id}` | — | Remover orçamento |
| | `POST` | `/api/Budgets/ConvertToOrder/{id}` | — | Converter em pedido |
| **Orders** | `GET` | `/api/Orders/GetAll` | — | Listar pedidos |
| | `GET` | `/api/Orders/GetById/{id}` | — | Buscar pedido |
| | `POST` | `/api/Orders/Create` | ✅ | Criar pedido |
| | `PUT` | `/api/Orders/Update/{id}` | ✅ | Atualizar pedido |
| | `DELETE` | `/api/Orders/Delete/{id}` | — | Remover pedido |
| **Receipts** | `GET` | `/api/Receipts/GetAll` | — | Listar recibos |
| | `GET` | `/api/Receipts/GetById/{id}` | — | Buscar recibo |
| | `POST` | `/api/Receipts/Create` | ✅ | Criar recibo |
| | `PUT` | `/api/Receipts/Update/{id}` | ✅ | Atualizar recibo |
| | `DELETE` | `/api/Receipts/Delete/{id}` | — | Remover recibo |
| **Expenses** | `GET` | `/api/Expenses/GetAll` | — | Listar despesas |
| | `GET` | `/api/Expenses/GetById/{id}` | — | Buscar despesa |
| | `POST` | `/api/Expenses/Create` | ✅ | Criar despesa |
| | `PUT` | `/api/Expenses/Update/{id}` | ✅ | Atualizar despesa |
| | `DELETE` | `/api/Expenses/Delete/{id}` | — | Remover despesa |
| **StockMovements** | `GET` | `/api/StockMovements/GetAll` | — | Listar movimentações |
| | `GET` | `/api/StockMovements/GetBySupplyId/{supplyId}` | — | Por insumo |
| | `GET` | `/api/StockMovements/GetByOrderId/{orderId}` | — | Por pedido |
| | `POST` | `/api/StockMovements/Create` | ✅ | Criar movimentação |

---

# Fluxo de Negócio Recomendado

```
1. Cadastrar Categorias
   POST /api/Categories/Create

2. Cadastrar Clientes
   POST /api/Customers/Create

3. Cadastrar Insumos no Estoque
   POST /api/Inventories/CreateSupply

4. Registrar Entrada de Estoque
   POST /api/StockMovements/Create  (type: 1 = Entrada)

5. Cadastrar Produtos Finais
   POST /api/Inventories/CreateFinalProduct

6. Criar Orçamento para cliente
   POST /api/Budgets/Create

7. Converter Orçamento em Pedido
   POST /api/Budgets/ConvertToOrder/{budgetId}

   OU criar pedido diretamente:
   POST /api/Orders/Create

8. Confirmar Pedido (gera saída de estoque automaticamente)
   PUT /api/Orders/Update/{orderId}  → { "status": 1 }

9. Concluir Pedido (gera recibo automaticamente)
   PUT /api/Orders/Update/{orderId}  → { "status": 3 }

10. Registrar Despesas
    POST /api/Expenses/Create
```

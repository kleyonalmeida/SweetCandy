# AGENTS.md — Domain/Enums/

Enums do domínio e seus valores:

Arquivos:
- FormaPagamento.cs — Dinheiro, Debito, Credito, Pix
- MovementType.cs — Entrada, Saida
- StatusOrder.cs — Pendente, Confirmada, Cancelada, Concluida
- Unidade.cs — Un, Kg, G, L, Ml, Mg, Caixa, Pacote

Uso:
- Usar `.IsInEnum()` nos validators.
- Mapear para DTOs conforme necessidade.

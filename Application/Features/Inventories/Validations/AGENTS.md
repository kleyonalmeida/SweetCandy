# AGENTS.md — Application/Features/Inventories/Validations/

Validators para Inventories.

Arquivos:
- CreateFinalProductCommandValidator.cs
- CreateSupplyCommandValidator.cs
- DeleteFinalProductCommandValidator.cs
- DeleteSupplyCommandValidator.cs
- UpdateFinalProductCommandValidator.cs
- UpdateSupplyCommandValidator.cs

Boas práticas:
- Validar FK (CategoryId, SupplyId) via serviço/contrato.
- Garantir quantidades >= 0.

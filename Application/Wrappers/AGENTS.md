# AGENTS.md — Application/Wrappers/

Contratos de resposta padronizados.

Arquivos:
- IReponseWrapper.cs
- ResponseWrapper.cs

Responsabilidade:
- Definir `IResponseWrapper` e genérico `IResponseWrapper<T>`.
- Fornecer funções helper estáticas para sucesso/falha (sync/async).

Regra:
- Handlers do MediatR devem retornar estes wrappers para que `ApiControllerBase` converta em HTTP adequadamente.

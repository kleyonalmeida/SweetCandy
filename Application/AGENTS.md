# AGENTS.md — Application/

Camada `Application` — CQRS, validators, DTOs, wrappers e mapeamentos.

Arquivos: Application.csproj
Subpastas: Common/, Docs/, Pipelines/, Wrappers/, Features/

Responsabilidade:
- Definir Commands/Queries e Handlers (MediatR).
- Validations com FluentValidation via `IValidateMe` + `ValidationPipelineBenaviour`.
- DTOs e mapeamentos Mapster.
- Definir interfaces de serviço que Infrastructure implementa.

Regras:
- Handlers retornam `IResponseWrapper` / `IResponseWrapper<T>`.
- Validadores não devem conter acesso a EF direto; usar serviços/contratos quando necessário.

# AGENTS.md — WebApi/

Projeto `WebApi` — configuração, DI e controllers.

Arquivos:
- Program.cs
- WebApi.csproj

Responsabilidade:
- Compor dependências (registrar serviços Infrastructure -> Application).
- Configurar EF Core/SQLite, Swagger, CORS e pipelines do MediatR.
- Servir SPA estática via `UseDefaultFiles` + `UseStaticFiles`.

Boas práticas:
- Registrar services como Scoped.
- Não implementar lógica de domínio nos controllers; apenas dispatch de commands/queries via `IMediator`.

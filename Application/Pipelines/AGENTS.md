# AGENTS.md — Application/Pipelines/

Pipeline de validação e contratos relacionados.

Arquivos:
- IValidateMe.cs — interface marcadora para ativar validação automática
- ValidationPipelineBenaviour.cs — `IPipelineBehavior` que executa validators e retorna `ResponseWrapper.FailAsync` em falhas

Fluxo:
1. Request chega (Command/Query) e implementa `IValidateMe` (opcional)
2. `ValidationPipelineBenaviour` resolve `IValidator<T>` e executa
3. Se erros → `ResponseWrapper.FailAsync(errors)`; se ok → handler

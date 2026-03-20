using Application.Wrappers;
using FluentValidation;
using MediatR;

namespace Application.Pipelines;

public class ValidationPipelineBenaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>, IValidateMe
{
  private readonly IEnumerable<IValidator<TRequest>> _validators;

  public ValidationPipelineBenaviour(IEnumerable<IValidator<TRequest>> validators)
  {
    _validators = validators;
  }

  public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
  {
    if (_validators != null && _validators.Any())
    {
      var context = new ValidationContext<TRequest>(request);
      var validationResults = await Task.WhenAll(_validators.Select(vr => vr.ValidateAsync(context, cancellationToken)));

      if (validationResults.Any(vr => !vr.IsValid))
      {
        var errors = new List<string>();

        var failures = validationResults.SelectMany(vr => vr.Errors)
            .Where(f => f != null)
            .ToList();

        foreach (var failure in failures)
        {
          if (!string.IsNullOrEmpty(failure.ErrorMessage))
            errors.Add(failure.ErrorMessage!);
        }

        var fail = await ResponseWrapper.FailAsync(errors);
        return (TResponse)(object)fail;
      }
    }
    return await next();
  }
}

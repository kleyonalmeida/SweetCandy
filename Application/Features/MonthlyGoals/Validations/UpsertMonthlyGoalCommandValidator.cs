using Application.Features.MonthlyGoals.Commands;
using FluentValidation;

namespace Application.Features.MonthlyGoals.Validations;

public class UpsertMonthlyGoalCommandValidator : AbstractValidator<UpsertMonthlyGoalCommand>
{
  public UpsertMonthlyGoalCommandValidator()
  {
    RuleFor(x => x.Request).NotNull();
    RuleFor(x => x.Request!.Year).InclusiveBetween(2000, 2100);
    RuleFor(x => x.Request!.Month).InclusiveBetween(1, 12);

    RuleFor(x => x.Request)
      .Must(r => r.TargetAmount.HasValue || r.PercentageOverCosts.HasValue)
      .WithMessage("Informe TargetAmount (R$) ou PercentageOverCosts (%).");

    RuleFor(x => x.Request!.TargetAmount)
      .GreaterThanOrEqualTo(0)
      .When(x => x.Request!.TargetAmount.HasValue);

    RuleFor(x => x.Request!.PercentageOverCosts)
      .InclusiveBetween(0, 1000)
      .When(x => x.Request!.PercentageOverCosts.HasValue);
  }
}

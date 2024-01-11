namespace AssociationRegistry.Admin.Api.Infrastructure.Validation;

using System;
using System.Linq.Expressions;
using FluentValidation;
using FluentValidation.Internal;
using Microsoft.AspNetCore.Http;
using System.Text.RegularExpressions;

public static class ValidatorHelpers
{
    public static void RequireNotNullOrEmpty<T>(this AbstractValidator<T> validator, Expression<Func<T, string?>> expression)
    {
        validator.RuleFor(expression)
            .NotNull()
            .WithMessage($"'{expression.GetMember().Name}' is verplicht.");
        validator.RuleFor(expression)
            .NotEmpty()
            .WithMessage($"'{expression.GetMember().Name}' mag niet leeg zijn.")
            .When(request => expression.Compile().Invoke(request) is { });
    }

    public static void RequireNotEmpty<T>(this AbstractValidator<T> validator, Expression<Func<T, string?>> expression)
    {
        validator.RuleFor(expression)
            .NotEmpty()
            .WithMessage($"'{expression.GetMember().Name}' mag niet leeg zijn.")
            .When(request => expression.Compile().Invoke(request) is { });
    }

    public static void RequireValidKboNummer<T>(this AbstractValidator<T> validator, Expression<Func<T, string?>> expression)
    {
        validator.RequireNotNullOrEmpty(expression);

        validator.RuleFor(expression)
            .Length(min: 10, int.MaxValue)
            .WithMessage($"'{expression.GetMember().Name}' moet 10 cijfers bevatten.")
            .When(request => !string.IsNullOrEmpty(expression.Compile().Invoke(request)));
    }

    public static IRuleBuilder<T, string?> MustNotContainHtml<T>(this IRuleBuilder<T, string?> ruleBuilder)
        => ruleBuilder
          .Must(NotContainHtml)
          .WithErrorCode(StatusCodes.Status400BadRequest.ToString())
          .WithMessage(ExceptionMessages.UnsupportedContent);

    private static bool NotContainHtml(string? propertyValue)
        => propertyValue is null || !Regex.IsMatch(propertyValue, pattern: "<.*?>");
}

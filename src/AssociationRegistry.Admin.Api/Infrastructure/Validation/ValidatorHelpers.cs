namespace AssociationRegistry.Admin.Api.Infrastructure.Validation;

using System;
using System.Linq.Expressions;
using FluentValidation;
using FluentValidation.Internal;

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
}

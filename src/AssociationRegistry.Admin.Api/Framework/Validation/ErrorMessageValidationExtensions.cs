namespace AssociationRegistry.Admin.Api.Framework.Validation;

using FluentValidation;

public static class ErrorMessageValidationExtensions
{
    public static IRuleBuilderOptions<T, TProperty> WithVeldIsVerplichtMessage<T, TProperty>(this IRuleBuilderOptions<T, TProperty> rule, string veld)
    => rule.WithMessage(string.Format(ValidationMessages.VeldIsVerplicht, veld));
}

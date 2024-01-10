namespace AssociationRegistry.Admin.Api.Infrastructure.Extensions;

using ExceptionHandlers;
using FluentValidation;
using HtmlValidation;
using JasperFx.Core.Reflection;
using Microsoft.AspNetCore.Http;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

public static class FluentValidatorExtensions
{
    public static async Task NullValidateAndThrowAsync<T>(
        this IValidator<T> validator,
        [NotNull] T? instance,
        CancellationToken cancellationToken = default)
    {
        if (instance is null) throw new CouldNotParseRequestException();

        await new NoHtmlValidator<T>().ValidateAndThrowAsync(instance, cancellationToken);
        await validator.ValidateAndThrowAsync(instance, cancellationToken);
    }
}

public class NoHtmlValidator<T> : AbstractValidator<T>
{
    private const string TO_BE_STRIPPED_PREFIX = "to-be-stripped-request-prefix";

    public NoHtmlValidator()
    {
        // TODO: nested
        // TODO: es kijken naar 'I <3 <Programming>' als waarde?
        RecursivelyApplyRule(typeof(T), TO_BE_STRIPPED_PREFIX);
    }

    private void RecursivelyApplyRule(Type type, string propertyName)
    {
        var props = type.GetProperties();

        foreach (var prop in props)
        {
            var currentPropertyName = $"{propertyName}.{prop.Name}";

            if (prop.HasAttribute<NoHtmlAttribute>())
            {
                ApplyRuleFor(prop, currentPropertyName);
            }
            else if (prop.PropertyType.IsArray)
            {
                if (prop.PropertyType.IsClass)
                    RecursivelyApplyRule(prop.PropertyType, currentPropertyName);
                else
                    ApplyRuleForEach(prop, currentPropertyName);
            }
            else if (prop.PropertyType.IsClass)
            {
                RecursivelyApplyRule(prop.PropertyType, currentPropertyName);
            }
        }
    }

    private void ApplyRuleFor(PropertyInfo prop, string propertyName)
    {
        if (prop.PropertyType == typeof(string))
            RuleFor(model => GetPropertyValue(model, prop.Name) as string)
               .Cascade(CascadeMode.Continue)
               .Must(BeNoHtml!)
               .WithName(propertyName.Replace($"{TO_BE_STRIPPED_PREFIX}.", newValue: ""))
               .WithErrorCode(StatusCodes.Status400BadRequest.ToString())
               .WithMessage(ExceptionMessages.UnsupportedContent)
               .When(model => GetPropertyValue(model, prop.Name) != null);
    }

    private void ApplyRuleForEach(PropertyInfo prop, string propertyName)
    {
        if (prop.PropertyType == typeof(string[]))
            RuleForEach(model => Convert.ChangeType(GetPropertyValue(model, prop.Name), prop.PropertyType) as string[])
               .Cascade(CascadeMode.Continue)
               .Must(BeNoHtml!)
               .WithName(propertyName.Replace($"{TO_BE_STRIPPED_PREFIX}.", newValue: ""))
               .WithErrorCode(StatusCodes.Status400BadRequest.ToString())
               .WithMessage(ExceptionMessages.UnsupportedContent)
               .When(model => GetPropertyValue(model, prop.Name) != null);
    }

    private static object? GetPropertyValue(T model, string propertyName)
    {
        var property = typeof(T).GetProperty(propertyName);

        return property?.GetValue(model);
    }

    private static bool BeNoHtml(string propertyValue)
        => !Regex.IsMatch(propertyValue, pattern: "<.*?>");
}

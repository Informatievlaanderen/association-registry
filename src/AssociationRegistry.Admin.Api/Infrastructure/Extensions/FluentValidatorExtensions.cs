namespace AssociationRegistry.Admin.Api.Infrastructure.Extensions;

using ExceptionHandlers;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using HtmlValidation;
using System.Linq;
using System.Text.RegularExpressions;

public static class FluentValidatorExtensions
{
    public static async Task NullValidateAndThrowAsync<T>(
        this IValidator<T> validator,
        [NotNull] T? instance,
        CancellationToken cancellationToken = default)
    {
        if (instance is null) throw new CouldNotParseRequestException();

        await new NoHtmlValidator<T>().ValidateAndThrowAsync(instance, cancellationToken: cancellationToken);
        await validator.ValidateAndThrowAsync(instance, cancellationToken);
    }
}

public class NoHtmlValidator<T> : AbstractValidator<T>
{
    public NoHtmlValidator()
    {
        ProcessNestedProperties(typeof(T), "");
    }

    private void ProcessNestedProperties(System.Type type, string parentPropertyName)
    {
        var propertiesWithNoHtml = type.GetProperties()
                                        .Where(p => p.GetCustomAttributes(typeof(NoHtmlAttribute), false).Any());

        foreach (var property in propertiesWithNoHtml)
        {
            string fullPropertyName = string.IsNullOrEmpty(parentPropertyName)
                                      ? property.Name
                                      : $"{parentPropertyName}.{property.Name}";

            if (property.PropertyType == typeof(string))
            {
                RuleFor(model => GetPropertyValue(model, fullPropertyName) as string)
                   .Must(BeNoHtml)
                   .WithMessage(ExceptionMessages.UnsupportedContent)
                   .When(model => GetPropertyValue(model, fullPropertyName) != null);
            }
            else if (property.PropertyType == typeof(string[]))
            {
                RuleForEach(model => GetPropertyValue(model, fullPropertyName) as string[])
                   .Must(BeNoHtml)
                   .WithName(property.Name)
                   .WithMessage(ExceptionMessages.UnsupportedContent)
                   .When(model => GetPropertyValue(model, fullPropertyName) != null);
            }
            else if (property.PropertyType.IsClass && property.PropertyType != typeof(string))
            {
                ProcessNestedProperties(property.PropertyType, fullPropertyName);
            }
        }
    }

    private static object? GetPropertyValue(T model, string propertyName)
    {
        var properties = propertyName.Split('.');
        var value = (object)model;

        foreach (var prop in properties)
        {
            var property = value.GetType().GetProperty(prop);
            value = property?.GetValue(value);

            if (value == null)
                return null;
        }

        return value;
    }
    private static bool BeNoHtml(string propertyValue)
        => !Regex.IsMatch(propertyValue, "<.*?>");
}

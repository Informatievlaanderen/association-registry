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
        var propertiesWithNoHtml = typeof(T).GetProperties()
                                            .Where(p => p.GetCustomAttributes(typeof(NoHtmlAttribute), false).Any());

        foreach (var property in propertiesWithNoHtml)
        {
            if (property.PropertyType == typeof(string))
            {
                RuleFor(model => GetPropertyValue(model, property.Name) as string)
                   .Must(BeNoHtml!)
                   .WithMessage(ExceptionMessages.UnsupportedContent)
                   .When(model => GetPropertyValue(model, property.Name) != null);
            }
            else if (property.PropertyType == typeof(string[]))
            {
                RuleForEach(model => GetPropertyValue(model, property.Name) as string[])
                   .Must(BeNoHtml!)
                   .WithMessage(ExceptionMessages.UnsupportedContent)
                   .When(model => GetPropertyValue(model, property.Name) != null);
            }
        }
    }

    private static object? GetPropertyValue(T model, string propertyName)
    {
        var property = typeof(T).GetProperty(propertyName);

        return property?.GetValue(model);
    }

    private static bool BeNoHtml(string propertyValue)
        => !Regex.IsMatch(propertyValue, "<.*?>");
}

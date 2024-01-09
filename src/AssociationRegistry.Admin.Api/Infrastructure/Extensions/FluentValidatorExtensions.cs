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
    public static async Task NullValidateAndThrowAsync<T>(this IValidator<T> validator, [NotNull] T? instance, CancellationToken cancellationToken = default)
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
            RuleFor(model => GetPropertyValue(model, property.Name))
               .Must(BeNoHtml)
               .WithMessage(ExceptionMessages.UnsupportedContent)
               .When(model => property.GetValue(model) != null);
        }
    }

    private static string GetPropertyValue(T model, string propertyName)
    {
        var property = typeof(T).GetProperty(propertyName);
        return property?.GetValue(model) as string;
    }

    private static bool BeNoHtml(string propertyValue)
        => Regex.IsMatch(propertyValue, "<.*?>");
}


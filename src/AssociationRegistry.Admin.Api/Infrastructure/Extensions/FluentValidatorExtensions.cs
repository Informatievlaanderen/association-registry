namespace AssociationRegistry.Admin.Api.Infrastructure.Extensions;

using ExceptionHandlers;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;

public static class FluentValidatorExtensions
{
    public static async Task NullValidateAndThrowAsync<T>(this IValidator<T> validator, [NotNull] T? instance, CancellationToken cancellationToken = default)
    {
        if (instance is null) throw new CouldNotParseRequestException();
        await validator.ValidateAndThrowAsync(instance, cancellationToken);
    }
}

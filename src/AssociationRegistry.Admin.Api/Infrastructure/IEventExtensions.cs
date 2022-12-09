namespace AssociationRegistry.Admin.Api.Infrastructure;

using System.Text.Json;
using Marten.Events;

public static class IEventExtensions
{
    public static string GetHeaderString(this IEvent source, string propertyName)
        => MaybeGetHeaderJsonElement(source, propertyName)
            ?.GetString() ?? string.Empty;

    private static JsonElement? MaybeGetHeaderJsonElement(this IEvent source, string propertyName)
        => !source.HasHeader(propertyName)
            ? null
            :(JsonElement)source.GetHeader(propertyName)!;

    private static bool HasHeader(this IEvent source, string propertyName)
        => source.Headers?.ContainsKey(propertyName) ?? false;
}

namespace AssociationRegistry.Framework;

using System.Collections.Generic;
using System.Text.Json;
using NodaTime;
using NodaTime.Text;

public static class IEventExtensions
{
    public static string GetHeaderString(this Marten.Events.IEvent source, string propertyName)
        => GetHeaderJsonElement(source, propertyName)
            .GetString()!;

    public static Instant GetHeaderInstant(this Marten.Events.IEvent source, string propertyName)
        => InstantPattern.General.Parse(source.GetHeaderString(propertyName)).GetValueOrThrow();

    private static JsonElement GetHeaderJsonElement(this Marten.Events.IEvent source, string propertyName)
        => source.HasHeader(propertyName)
            ? throw new KeyNotFoundException()
            : (JsonElement)source.GetHeader(propertyName)!;

    private static bool HasHeader(this Marten.Events.IEvent source, string propertyName)
        => !source.Headers?.ContainsKey(propertyName) ?? false;
}

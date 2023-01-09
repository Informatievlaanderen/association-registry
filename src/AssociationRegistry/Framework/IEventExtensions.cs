namespace AssociationRegistry.Framework;

using System;
using System.Collections.Generic;
using System.Text.Json;
using NodaTime;

public static class IEventExtensions
{
    public static string GetHeaderString(this Marten.Events.IEvent source, string propertyName)
        => GetHeaderJsonElement(source, propertyName)
            .GetString()!;

    public static Instant GetHeaderInstant(this Marten.Events.IEvent source, string propertyName)
        => Instant.FromDateTimeOffset(
            DateTimeOffset.Parse(
                GetHeaderJsonElement(source, propertyName)
                    .GetString() ?? throw new InvalidOperationException()));

    private static JsonElement GetHeaderJsonElement(this Marten.Events.IEvent source, string propertyName)
        => source.HasHeader(propertyName)
            ? throw new KeyNotFoundException()
                :(JsonElement)source.GetHeader(propertyName)!;

    private static bool HasHeader(this Marten.Events.IEvent source, string propertyName)
        => !source.Headers?.ContainsKey(propertyName) ?? false;
}

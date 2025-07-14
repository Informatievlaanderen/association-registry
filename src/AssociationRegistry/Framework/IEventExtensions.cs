namespace AssociationRegistry.Framework;

using NodaTime;
using NodaTime.Text;
using System.Text.Json;

public static class IEventExtensions
{
    public static string GetHeaderString(this JasperFx.Events.IEvent source, string propertyName)
        => GetHeaderJsonElement(source, propertyName);

    public static Instant GetHeaderInstant(this JasperFx.Events.IEvent source, string propertyName)
        => InstantPattern.General.Parse(source.GetHeaderString(propertyName)).GetValueOrThrow();

    private static string GetHeaderJsonElement(this JasperFx.Events.IEvent source, string propertyName)
    {
        if(!source.HasHeader(propertyName))
            throw new KeyNotFoundException();

        if (source.GetHeader(propertyName) is JsonElement)
        {
            return ((JsonElement)source.GetHeader(propertyName)!).GetString()!;
        }
        else
        {
            return (string)source.GetHeader(propertyName);
        }
    }

    private static bool HasHeader(this JasperFx.Events.IEvent source, string propertyName)
        => source.Headers?.Keys.Contains(propertyName) ?? false;
}

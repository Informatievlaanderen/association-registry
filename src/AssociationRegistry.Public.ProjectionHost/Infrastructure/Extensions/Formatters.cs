namespace AssociationRegistry.Public.ProjectionHost.Infrastructure.Extensions;

using Events;

public static class Formatters
{
    public static string ToAdresString(this VerenigingWerdGeregistreerd.Locatie locatie)
        => $"{locatie.Straatnaam} {locatie.Huisnummer}" +
           (locatie.Busnummer is not null ? $" bus {locatie.Busnummer}" : string.Empty) +
           $", {locatie.Postcode} {locatie.Gemeente}, {locatie.Land}";
}

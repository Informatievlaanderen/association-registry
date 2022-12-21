namespace AssociationRegistry.Public.Api.Constants;

using Vereniging;

public static class Formaters
{
    public static string ToAdresString(this VerenigingWerdGeregistreerd.Locatie locatie)
        => $"{locatie.Straatnaam} {locatie.Huisnummer}" +
           (locatie.Busnummer is not null ? $" bus {locatie.Busnummer}" : string.Empty) +
           $", {locatie.Postcode} {locatie.Gemeente}, {locatie.Land}";
}

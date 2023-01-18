﻿namespace AssociationRegistry.Admin.Api.Infrastructure.Extensions;

using Events;

public static class Formaters
{
    public static string ToAdresString(this VerenigingWerdGeregistreerd.Locatie locatie)
        => $"{locatie.Straatnaam} {locatie.Huisnummer}" +
           (locatie.Busnummer is not null ? $" bus {locatie.Busnummer}" : string.Empty) +
           $", {locatie.Postcode} {locatie.Gemeente}, {locatie.Land}";
}

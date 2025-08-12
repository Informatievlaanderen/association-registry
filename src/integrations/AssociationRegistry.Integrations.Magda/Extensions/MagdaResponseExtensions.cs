namespace AssociationRegistry.Integrations.Magda.Extensions;

using Models;
using Models.GeefOnderneming;
using System;
using System.Linq;
using UitzonderingType = Onderneming.GeefOnderneming.UitzonderingType;
using UitzonderingTypeType = Onderneming.GeefOnderneming.UitzonderingTypeType;

public static class MagdaResponseExtensions
{
    public static string ConcatenateUitzonderingen(
        this UitzonderingType[] source,
        string separator,
        UitzonderingTypeType uitzonderingTypeType)
        => string.Join(
            separator,
            source.Where(type => type.Type == uitzonderingTypeType)
                  .Select(type => $"{type.Identificatie} - {type.Diagnose} - {type.Omstandigheid}"));

    public static string ConcatenateUitzonderingen(
        this AssociationRegistry.Integrations.Magda.Repertorium.RegistreerInschrijving.UitzonderingType[] source,
        string separator,
        AssociationRegistry.Integrations.Magda.Repertorium.RegistreerInschrijving.UitzonderingTypeType uitzonderingTypeType)
        => string.Join(
            separator,
            source.Where(type => type.Type == uitzonderingTypeType)
                  .Select(type => $"{type.Identificatie} - {type.Diagnose} - {type.Omstandigheid}"));

    public static bool HasUitzonderingenOfType(
        this ResponseEnvelope<GeefOndernemingResponseBody>? source,
        UitzonderingTypeType uitzonderingTypeType)
    {
        return HasAnyUitzondering(source, predicate: type => type.Type == uitzonderingTypeType);
    }

    public static bool HasAnyUitzondering(
        this ResponseEnvelope<GeefOndernemingResponseBody>? source,
        Func<UitzonderingType, bool> predicate)
        => source?.Body?.GeefOndernemingResponse?.Repliek?.Antwoorden?.Antwoord?.Uitzonderingen?.Any(predicate) ?? false;
}

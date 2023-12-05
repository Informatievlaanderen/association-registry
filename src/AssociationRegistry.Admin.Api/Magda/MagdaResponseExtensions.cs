namespace AssociationRegistry.Admin.Api.Magda;

using AssociationRegistry.Magda.Models;
using AssociationRegistry.Magda.Models.GeefOnderneming;
using AssociationRegistry.Magda.Onderneming.GeefOnderneming;
using System;
using System.Linq;

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

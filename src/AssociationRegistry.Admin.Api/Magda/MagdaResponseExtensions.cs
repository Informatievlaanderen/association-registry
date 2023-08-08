namespace AssociationRegistry.Admin.Api.Magda;

using System;
using System.Linq;
using AssociationRegistry.Magda.Models;
using AssociationRegistry.Magda.Models.GeefOnderneming;
using AssociationRegistry.Magda.Onderneming.GeefOnderneming;

public static class MagdaResponseExtensions
{
    public static string ConcatenateUitzonderingen(this UitzonderingType[] source, string separator)
        => string.Join(
            separator,
            source.Where(type => type.Type == UitzonderingTypeType.FOUT)
                .Select(type => type.Diagnose));

    public static bool HasUitzonderingenOfType(this ResponseEnvelope<GeefOndernemingResponseBody>? source, UitzonderingTypeType uitzonderingTypeType)
    {
        return HasAnyUitzondering(source, type => type.Type == uitzonderingTypeType);
    }

    public static bool HasAnyUitzondering(this ResponseEnvelope<GeefOndernemingResponseBody>? source, Func<UitzonderingType, bool> predicate)
        => source?.Body?.GeefOndernemingResponse?.Repliek?.Antwoorden?.Antwoord?.Uitzonderingen?.Any(predicate) ?? false;
}

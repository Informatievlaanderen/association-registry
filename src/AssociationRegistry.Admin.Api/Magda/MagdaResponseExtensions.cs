namespace AssociationRegistry.Admin.Api.Magda;

using System.Diagnostics.CodeAnalysis;
using System.Linq;
using AssociationRegistry.Magda.Models;
using AssociationRegistry.Magda.Onderneming.GeefOndernemingVKBO;

public static class MagdaResponseExtensions
{
    public static string ConcatenateUitzonderingen(this UitzonderingType[] source, string separator)
        => string.Join(
            separator,
            source.Where(type => type.Type == UitzonderingTypeType.FOUT)
                .Select(type => type.Diagnose));

    public static bool HasUitzonderingenOfTypes([NotNullWhen(false)] this ResponseEnvelope<GeefOndernemingResponseBody>? source, UitzonderingTypeType uitzonderingTypeType)
    {
        return source?.Body?.GeefOndernemingVKBOResponse?.Repliek?.Antwoorden?.Antwoord?.Uitzonderingen?.Any(type => type.Type == uitzonderingTypeType) ?? false;
    }
}

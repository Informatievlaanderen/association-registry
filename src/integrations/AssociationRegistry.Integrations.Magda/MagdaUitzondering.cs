namespace AssociationRegistry.Integrations.Magda;

using System.Diagnostics.CodeAnalysis;

public record MagdaUitzondering(string Identificatie, string Omschrijving, bool IsBlokkerend)
{
    public static readonly MagdaUitzondering OnbekendKboNummer =
        new(Identificatie: "40046", Omschrijving: "Gevraagde ondernemingsnummer bestaat niet in KBO", IsBlokkerend: true);

    public static readonly MagdaUitzondering[] All = { OnbekendKboNummer };

    public static bool CanParse(string? value)
    {
        if (value is null)
            return false;

        return Array.Find(All, match: candidate => candidate.Identificatie == value) is not null;
    }

    public static bool TryParse(string? value, [NotNullWhen(true)] out MagdaUitzondering? parsed)
    {
        parsed = null;

        if (value is null)
            return false;

        parsed = Array.Find(All, match: candidate => candidate.Identificatie == value) ?? null;

        return parsed is not null;
    }

    public static MagdaUitzondering Parse(string value)
    {
        if (value == null)
            throw new ArgumentNullException(nameof(value));

        if (!TryParse(value, out var parsed))
            throw new FormatException($"De waarde {value} is geen gekend contactgegeven type.");

        return parsed;
    }

    public static bool IsGekendBlokkerendeUitzondering(string identificatie)
        => TryParse(identificatie, out var magdaUitzondering)
        && magdaUitzondering.IsBlokkerend;
}

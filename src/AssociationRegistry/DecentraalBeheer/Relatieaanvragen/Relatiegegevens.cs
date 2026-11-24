namespace AssociationRegistry.DecentraalBeheer.Relatieaanvragen;

/// <summary>
///     The optional identificatie and beschrijving of the relatie that a confirmation makes: of the lidmaatschap, or of
///     the subvereniging. Empty when not given, like the existing lidmaatschap and subtype endpoints.
/// </summary>
public sealed record Relatiegegevens
{
    public static readonly Relatiegegevens Empty = new(string.Empty, string.Empty);

    private Relatiegegevens(string identificatie, string beschrijving)
    {
        Identificatie = identificatie;
        Beschrijving = beschrijving;
    }

    public string Identificatie { get; }
    public string Beschrijving { get; }

    public static Relatiegegevens Create(string? identificatie, string? beschrijving) =>
        new(identificatie ?? string.Empty, beschrijving ?? string.Empty);
}

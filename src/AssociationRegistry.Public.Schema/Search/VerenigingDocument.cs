namespace AssociationRegistry.Public.Schema.Search;

public class VerenigingDocument
{
    public record Locatie(
        string Locatietype,
        string? Naam,
        string Adres,
        bool Hoofdlocatie,
        string Postcode,
        string Gemeente);

    public record HoofdactiviteitVerenigingsloket(
        string Code,
        string Naam);

    public string VCode { get; set; } = null!;
    public string Naam { get; set; } = null!;
    public string KorteNaam { get; set; } = null!;
    public Locatie[] Locaties { get; set; } = null!;
    public HoofdactiviteitVerenigingsloket[] HoofdactiviteitenVerenigingsloket { get; set; } = null!;
    public string Doelgroep { get; set; } = null!;
    public string[] Activiteiten { get; set; } = null!;

}

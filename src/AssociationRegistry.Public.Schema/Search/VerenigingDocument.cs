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

    public record Hoofdactiviteit(
        string Code,
        string Naam);

    public string VCode { get; set; } = null!;
    public string Naam { get; set; } = null!;
    public string? KorteNaam { get; set; }
    public Locatie[] Locaties { get; set; } = null!;
    public Hoofdactiviteit[] Hoofdactiviteiten { get; set; } = null!;
    public string Doelgroep { get; set; } = null!;
    public string[] Activiteiten { get; set; } = null!;

}

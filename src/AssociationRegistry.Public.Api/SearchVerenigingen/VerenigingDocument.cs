namespace AssociationRegistry.Public.Api.SearchVerenigingen;

public record VerenigingDocument(
    string VCode,
    string Naam,
    string? KorteNaam,
    VerenigingDocument.Locatie Hoofdlocatie,
    VerenigingDocument.Locatie[] Locaties,
    VerenigingDocument.Hoofdactiviteit[] Hoofdactiviteiten,
    string Doelgroep,
    string[] Activiteiten
)
{
    public record Locatie(
        string Postcode,
        string Gemeente);

    public record Hoofdactiviteit(
        string Code,
        string Naam);
}

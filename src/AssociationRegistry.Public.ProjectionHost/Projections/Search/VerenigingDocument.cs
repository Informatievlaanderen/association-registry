namespace AssociationRegistry.Public.ProjectionHost.Projections.Search;

public record VerenigingDocument(
    string VCode,
    string Naam,
    string? KorteNaam,
    VerenigingDocument.Locatie[] Locaties,
    VerenigingDocument.Hoofdactiviteit[] Hoofdactiviteiten,
    string Doelgroep,
    string[] Activiteiten
)
{
    public record Locatie(
        string Type,
        string? Naam,
        string Adres,
        bool Hoofdlocatie,
        string Postcode,
        string Gemeente);

    public record Hoofdactiviteit(
        string Code,
        string Naam);
}

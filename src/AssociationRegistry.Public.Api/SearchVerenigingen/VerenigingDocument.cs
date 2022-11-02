namespace AssociationRegistry.Public.Api.SearchVerenigingen;

public record VerenigingDocument(
    string VCode,
    string Naam,
    string KorteNaam,
    VerenigingDocument.Locatie Hoofdlocatie,
    VerenigingDocument.Locatie[] Locaties,
    string[] Hoofdactiviteiten,
    string Doelgroep,
    string[] Activiteiten
)
{
    public record Locatie(
        string Postcode,
        string Gemeente);
}

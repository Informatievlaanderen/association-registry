namespace AssociationRegistry.Public.Api.SearchVerenigingen;

using Nest;

public record VerenigingDocument(
    [property: Keyword] string VCode,
    [property: Text] string Naam,
    [property: Text] string KorteNaam,
    [property: Text] string Hoofdlocatie,
    [property: Text] string AndereLocaties,
    [property: Text] string PROTPUT,
    [property: Text] string Doelgroep
    );

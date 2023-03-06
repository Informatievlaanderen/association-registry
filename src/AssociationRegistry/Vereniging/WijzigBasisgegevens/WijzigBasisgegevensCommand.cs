namespace AssociationRegistry.Vereniging.WijzigBasisgegevens;

public record WijzigBasisgegevensCommand(
    string VCode,
    string? Naam = null,
    string? KorteNaam = null,
    string? KorteBeschrijving = null,
    DateOnly? StartDatum = null);

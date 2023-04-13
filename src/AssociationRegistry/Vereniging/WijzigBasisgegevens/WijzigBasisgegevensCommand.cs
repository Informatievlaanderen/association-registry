namespace AssociationRegistry.Vereniging.WijzigBasisgegevens;

using Startdatums;
using VCodes;
using VerenigingsNamen;

public record WijzigBasisgegevensCommand(
    VCode VCode,
    VerenigingsNaam? Naam = null,
    string? KorteNaam = null,
    string? KorteBeschrijving = null,
    Startdatum? Startdatum = default);

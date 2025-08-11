namespace AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Basisgegevens.VerenigingMetRechtspersoonlijkheid;

using AssociationRegistry.DecentraalBeheer.Vereniging;

public record WijzigBasisgegevensCommand(
    VCode VCode,
    string? Roepnaam = null,
    string? KorteBeschrijving = null,
    Doelgroep? Doelgroep = null,
    HoofdactiviteitVerenigingsloket[]? HoofdactiviteitenVerenigingsloket = null,
    Werkingsgebied[]? Werkingsgebieden = null);

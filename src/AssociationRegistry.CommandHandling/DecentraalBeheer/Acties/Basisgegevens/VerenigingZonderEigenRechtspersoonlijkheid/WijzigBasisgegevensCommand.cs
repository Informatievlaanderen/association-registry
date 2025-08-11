namespace AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Basisgegevens.VerenigingZonderEigenRechtspersoonlijkheid;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.Primitives;

public record WijzigBasisgegevensCommand(
    VCode VCode,
    VerenigingsNaam? Naam = null,
    string? KorteNaam = null,
    string? KorteBeschrijving = null,
    NullOrEmpty<Datum> Startdatum = default,
    Doelgroep? Doelgroep = null,
    HoofdactiviteitVerenigingsloket[]? HoofdactiviteitenVerenigingsloket = null,
    Werkingsgebied[]? Werkingsgebieden = null,
    bool? IsUitgeschrevenUitPubliekeDatastroom = null);

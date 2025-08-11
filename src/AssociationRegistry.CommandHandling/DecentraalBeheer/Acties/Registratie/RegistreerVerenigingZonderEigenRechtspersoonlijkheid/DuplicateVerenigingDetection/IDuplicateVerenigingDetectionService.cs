namespace AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Registratie.RegistreerVerenigingZonderEigenRechtspersoonlijkheid.DuplicateVerenigingDetection;

using AssociationRegistry.DecentraalBeheer.Vereniging;

public interface IDuplicateVerenigingDetectionService
{
    Task<IReadOnlyCollection<DuplicaatVereniging>> ExecuteAsync(
        VerenigingsNaam naam,
        Locatie[] locaties,
        bool includeScore = false,
        MinimumScore? minimumScoreOverride = null);

    Task<IReadOnlyCollection<DuplicaatVereniging>> ExecuteAsync(
        VerenigingsNaam naam,
        DuplicateVerenigingZoekQueryLocaties locaties,
        bool includeScore = false,
        MinimumScore? minimumScoreOverride = null);
}

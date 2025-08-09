namespace AssociationRegistry.DecentraalBeheer.Acties.Registratie.RegistreerVerenigingZonderEigenRechtspersoonlijkheid.DuplicateVerenigingDetection;

using Vereniging;

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

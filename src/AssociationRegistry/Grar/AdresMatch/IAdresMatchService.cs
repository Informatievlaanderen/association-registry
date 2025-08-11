namespace AssociationRegistry.Grar.AdresMatch;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.Events;

public interface IAdresMatchService
{
    Task<IEvent> GetAdresMatchEvent(
        int locatieId,
        Locatie? locatie,
        VCode vCode,
        CancellationToken cancellationToken);

    Task<AdresMatchResult> ProcessAdresMatch(
        AdresMatchRequest request,
        CancellationToken cancellationToken);
}

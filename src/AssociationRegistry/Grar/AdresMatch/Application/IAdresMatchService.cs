namespace AssociationRegistry.Grar.AdresMatch.Application;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.Events;
using AssociationRegistry.Vereniging;
using Domain;

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

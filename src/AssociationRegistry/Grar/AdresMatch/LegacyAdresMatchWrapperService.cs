namespace AssociationRegistry.Grar.AdresMatch;

using Clients;
using DecentraalBeheer.Vereniging;
using Events;
using Vereniging;

public class LegacyAdresMatchWrapperService
{
    // Static method preserved for backward compatibility
    // This acts as an adapter to the new refactored implementation
    public static async Task<IEvent> GetAdresMatchEvent(
        int locatieId,
        Locatie locatie,
        IGrarClient grarClient,
        CancellationToken cancellationToken,
        VCode vCode)
    {
        // Create the services inline for backward compatibility
        var matchStrategy = new PerfectScoreMatchStrategy();
        var verrijkingService = new GemeenteVerrijkingService(grarClient);
        var service = new AdresMatchService(grarClient, matchStrategy, verrijkingService);

        return await service.GetAdresMatchEvent(locatieId, locatie, vCode, cancellationToken);
    }
}

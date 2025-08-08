namespace AssociationRegistry.Grar.AdresMatch;

using Application;
using Clients;
using DecentraalBeheer.Vereniging;
using Domain;
using Events;
using Infrastructure;
using Vereniging;

public class AdresMatchService
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
        var service = new AdresMatchServiceRefactored(grarClient, matchStrategy, verrijkingService);
        
        return await service.GetAdresMatchEvent(locatieId, locatie, vCode, cancellationToken);
    }
}

namespace AssociationRegistry.CommandHandling.Grar.GrarConsumer.Messaging.HeradresseerLocaties;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.Framework;
using AssociationRegistry.Grar.Models;
using AssociationRegistry.Integrations.Grar.Clients;
using GrarUpdates.Hernummering;
using Integrations.Grar.AdresMatch;
using MartenDb.Store;

public class HeradresseerLocatiesMessageHandler
{
    private readonly IAggregateSession _aggregateSession;
    private readonly IGrarClient _client;

    public HeradresseerLocatiesMessageHandler(IAggregateSession aggregateSession, IGrarClient client)
    {
        _aggregateSession = aggregateSession;
        _client = client;
    }

    public async Task Handle(HeradresseerLocatiesMessage doorFusieMessage, CancellationToken cancellationToken)
    {
        var metadata = CommandMetadata.ForDigitaalVlaanderenProcess;
        var vereniging = await _aggregateSession.Load<VerenigingOfAnyKind>(
            VCode.Hydrate(doorFusieMessage.VCode),
            metadata,
            allowDubbeleVereniging: true
        );

        var locatiesWithAddresses = await FetchAddressesForLocaties(
            doorFusieMessage.TeHeradresserenLocaties,
            cancellationToken
        );

        await vereniging.HeradresseerLocaties(
            locatiesWithAddresses,
            doorFusieMessage.idempotencyKey,
            new GrarAddressVerrijkingsService(_client)
        );

        await _aggregateSession.Save(
            vereniging,
            metadata with
            {
                ExpectedVersion = vereniging.Version,
            },
            cancellationToken
        );
    }

    private async Task<List<LocatieWithAdres>> FetchAddressesForLocaties(
        List<TeHeradresserenLocatie> locatiesMetAdres,
        CancellationToken cancellationToken
    )
    {
        var locatiesWithAddresses = new List<LocatieWithAdres>();

        foreach (var (locatieId, adresId) in locatiesMetAdres)
        {
            var adres = await _client.GetAddressById(adresId, cancellationToken);
            locatiesWithAddresses.Add(new LocatieWithAdres(locatieId, adres));
        }

        return locatiesWithAddresses;
    }
}

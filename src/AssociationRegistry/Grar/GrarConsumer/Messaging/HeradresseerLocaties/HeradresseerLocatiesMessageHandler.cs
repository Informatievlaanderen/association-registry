namespace AssociationRegistry.Grar.GrarConsumer.Messaging.HeradresseerLocaties;

using AssociationRegistry.EventStore;
using AssociationRegistry.Framework;
using AssociationRegistry.Grar.GrarUpdates.Hernummering;
using AssociationRegistry.Vereniging;
using Clients;
using Models;
using NodaTime;

public class HeradresseerLocatiesMessageHandler
{
    private readonly IVerenigingsRepository _repository;
    private readonly IGrarClient _client;

    public HeradresseerLocatiesMessageHandler(IVerenigingsRepository repository, IGrarClient client)
    {
        _repository = repository;
        _client = client;
    }

    public async Task Handle(HeradresseerLocatiesMessage doorFusieMessage, CancellationToken cancellationToken)
    {
        var vereniging = await _repository.Load<VerenigingOfAnyKind>(VCode.Hydrate(doorFusieMessage.VCode), allowDubbeleVereniging: true);

        var locatiesWithAddresses = await FetchAddressesForLocaties(doorFusieMessage.TeHeradresserenLocaties, cancellationToken);

        await vereniging.HeradresseerLocaties(locatiesWithAddresses, doorFusieMessage.idempotencyKey, _client);

        await _repository.Save(vereniging, new CommandMetadata(EventStore.DigitaalVlaanderenOvoNumber,
                                                               SystemClock.Instance.GetCurrentInstant(), Guid.NewGuid(),
                                                               vereniging.Version), cancellationToken);
    }

    private async Task<List<LocatieWithAdres>> FetchAddressesForLocaties(List<TeHeradresserenLocatie> locatiesMetAdres, CancellationToken cancellationToken)
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

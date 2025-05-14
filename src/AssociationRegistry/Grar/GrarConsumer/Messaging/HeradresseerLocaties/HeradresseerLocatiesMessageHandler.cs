namespace AssociationRegistry.Grar.GrarConsumer.Messaging.HeradresseerLocaties;

using AssociationRegistry.Framework;
using AssociationRegistry.Grar.GrarUpdates.Hernummering;
using AssociationRegistry.Vereniging;
using Clients;
using Models;

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
        var metadata = CommandMetadata.ForDigitaalVlaanderenProcess;
        var vereniging = await _repository.Load<VerenigingOfAnyKind>(VCode.Hydrate(doorFusieMessage.VCode), metadata, allowDubbeleVereniging: true);

        var locatiesWithAddresses = await FetchAddressesForLocaties(doorFusieMessage.TeHeradresserenLocaties, cancellationToken);

        await vereniging.HeradresseerLocaties(locatiesWithAddresses, doorFusieMessage.idempotencyKey, _client);

        await _repository.Save(vereniging, metadata with { ExpectedVersion = vereniging.Version }, cancellationToken);
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

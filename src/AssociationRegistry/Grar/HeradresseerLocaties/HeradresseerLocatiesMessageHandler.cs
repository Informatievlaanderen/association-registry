namespace AssociationRegistry.Grar.HeradresseerLocaties;

using EventStore;
using Framework;
using Models;
using NodaTime;
using Vereniging;

public class HeradresseerLocatiesMessageHandler
{
    private readonly IVerenigingsRepository _repository;
    private readonly IGrarClient _client;

    public HeradresseerLocatiesMessageHandler(IVerenigingsRepository repository, IGrarClient client)
    {
        _repository = repository;
        _client = client;
    }

    public async Task Handle(TeHeradresserenLocatiesMessage message, CancellationToken cancellationToken)
    {
        var vereniging = await _repository.Load<VerenigingOfAnyKind>(VCode.Hydrate(message.VCode));

        var locatiesWithAddresses = await FetchAddressesForLocaties(message.LocatiesMetAdres);

        vereniging.HeradresseerLocaties(locatiesWithAddresses, message.idempotencyKey);

        await _repository.Save(vereniging, new CommandMetadata(EventStore.DigitaalVlaanderenOvoNumber,
                                                               SystemClock.Instance.GetCurrentInstant(), Guid.NewGuid(),
                                                               vereniging.Version), cancellationToken);
    }

    private async Task<List<LocatieWithAdres>> FetchAddressesForLocaties(List<LocatieIdWithAdresId> locatiesMetAdres)
    {
        var locatiesWithAddresses = new List<LocatieWithAdres>();

        foreach (var (locatieId, adresId) in locatiesMetAdres)
        {
            var adres = await _client.GetAddress(adresId);
            locatiesWithAddresses.Add(new LocatieWithAdres(locatieId, adres));
        }

        return locatiesWithAddresses;
    }
}

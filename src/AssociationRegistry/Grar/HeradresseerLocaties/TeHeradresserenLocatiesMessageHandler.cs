namespace AssociationRegistry.Grar.HeradresseerLocaties;

using EventStore;
using Framework;
using Models;
using NodaTime;
using Vereniging;

public class TeHeradresserenLocatiesMessageHandler
{
    private readonly IVerenigingsRepository _repository;
    private readonly IGrarClient _client;

    public TeHeradresserenLocatiesMessageHandler(IVerenigingsRepository repository, IGrarClient client)
    {
        _repository = repository;
        _client = client;
    }

    public async Task Handle(TeHeradresserenLocatiesMessage message, CancellationToken cancellationToken)
    {
        var vereniging = await _repository.Load<VerenigingOfAnyKind>(VCode.Hydrate(message.VCode));

        var locatiesWithAddresses = await FetchAddressesForLocaties(message.LocatiesMetAdres, cancellationToken);

        await vereniging.HeradresseerLocaties(locatiesWithAddresses, message.idempotencyKey, _client);

        await _repository.Save(vereniging, new CommandMetadata(EventStore.DigitaalVlaanderenOvoNumber,
                                                               SystemClock.Instance.GetCurrentInstant(), Guid.NewGuid(),
                                                               vereniging.Version), cancellationToken);
    }

    private async Task<List<LocatieWithAdres>> FetchAddressesForLocaties(List<LocatieIdWithAdresId> locatiesMetAdres, CancellationToken cancellationToken)
    {
        var locatiesWithAddresses = new List<LocatieWithAdres>();

        foreach (var (locatieId, adresId) in locatiesMetAdres)
        {
            // if adresid is not null
            var adres = await _client.GetAddressById(adresId, cancellationToken);
            locatiesWithAddresses.Add(new LocatieWithAdres(locatieId, adres));
        }

        return locatiesWithAddresses;
    }
}

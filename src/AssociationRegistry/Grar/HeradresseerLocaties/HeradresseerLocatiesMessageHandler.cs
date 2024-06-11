namespace AssociationRegistry.Grar.HeradresseerLocaties;

using AssociationRegistry.Framework;
using AssociationRegistry.Grar;
using AssociationRegistry.Grar.Models;
using AssociationRegistry.Vereniging;
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

    public async Task Handle(TeHeradresserenLocatiesMessage message)
    {
        var vereniging = await _repository.Load<VerenigingOfAnyKind>(VCode.Hydrate(message.VCode));

        var locatiesWithAddresses = await FetchAddressesForLocaties(message.LocatiesMetAdres);

        vereniging.HeradresseerLocatie(locatiesWithAddresses, message.idempotencyKey);

        await _repository.Save(vereniging, new CommandMetadata("", Instant.MinValue, Guid.NewGuid()), CancellationToken.None);
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

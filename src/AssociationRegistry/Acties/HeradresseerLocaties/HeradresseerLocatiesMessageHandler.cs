namespace AssociationRegistry.Acties.HeradresseerLocaties;

using Framework;
using Grar;
using Grar.Models;
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

    public async Task Handle(TeHeradresserenLocatiesMessage message)
    {
        var vereniging = await _repository.Load<VerenigingOfAnyKind>(VCode.Hydrate(message.VCode));

        var locatiesWithAddresses = await FetchAddressesForLocaties(message.LocatiesMetAdres);

        vereniging.HeradresseerLocatie(locatiesWithAddresses, message.idempotencyKey);

        await _repository.Save(vereniging, new CommandMetadata("", Instant.MinValue, Guid.NewGuid()), CancellationToken.None);
    }

    private async Task<List<(int, AddressDetailResponse)>> FetchAddressesForLocaties(List<(int, string)> locatiesMetAdres)
    {
        var locatiesWithAddresses = new List<(int, AddressDetailResponse)>();

        foreach (var (locatieId, adresId) in locatiesMetAdres)
        {
            var adres = await _client.GetAddress(adresId);
            locatiesWithAddresses.Add((locatieId, adres));
        }

        return locatiesWithAddresses;
    }
}

namespace AssociationRegistry.Acties.HeradresseerLocaties;

using Framework;
using Grar;
using NodaTime;
using Vereniging;

public class HeradresseerLocatiesMessageHandler
{
    public async Task Handle(TeHeradresserenLocatiesMessage message, IVerenigingsRepository repository, IGrarClient client)
    {
        var vereniging = await repository.Load<VerenigingOfAnyKind>(VCode.Hydrate(message.VCode));

        foreach (var (locatieId, adresId) in message.LocatiesMetAdres) // TODO: oud en nieuw adres id, of iets anders voor idempotency.
        {
            var adres = await client.GetAddress(adresId);

            vereniging.HeradresseerLocatie(locatieId, adres);
        }

        await repository.Save(vereniging, new CommandMetadata("", Instant.MinValue, Guid.NewGuid()), CancellationToken.None);
    }
}

namespace AssociationRegistry.Acties.GrarConsumer.OntkoppelAdres;

using AssociationRegistry.EventStore;
using AssociationRegistry.Framework;
using AssociationRegistry.Vereniging;
using NodaTime;

public class OntkoppelLocatiesMessageHandler
{
    private readonly IVerenigingsRepository _repository;

    public OntkoppelLocatiesMessageHandler(IVerenigingsRepository repository)
    {
        _repository = repository;
    }

    public async Task Handle(OntkoppelLocatiesMessage message, CancellationToken cancellationToken)
    {
        var vereniging = await _repository.Load<VerenigingOfAnyKind>(VCode.Hydrate(message.VCode));

        foreach (var teOntkoppelenLocatieId in message.TeOntkoppelenLocatieIds)
        {
            vereniging.OntkoppelLocatie(teOntkoppelenLocatieId);
        }

        await _repository.Save(vereniging, new CommandMetadata(EventStore.DigitaalVlaanderenOvoNumber,
                                                               SystemClock.Instance.GetCurrentInstant(), Guid.NewGuid(),
                                                               vereniging.Version), cancellationToken);
    }
}

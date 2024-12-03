namespace AssociationRegistry.Acties.OntkoppelAdres;

using AssociationRegistry.EventStore;
using AssociationRegistry.Framework;
using AssociationRegistry.Vereniging;
using NodaTime;

public class OntkoppelLocatiesHandler
{
    private readonly IVerenigingsRepository _repository;

    public OntkoppelLocatiesHandler(IVerenigingsRepository repository)
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

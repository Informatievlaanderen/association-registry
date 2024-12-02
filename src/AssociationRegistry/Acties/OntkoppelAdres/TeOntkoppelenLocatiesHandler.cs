namespace AssociationRegistry.Acties.OntkoppelAdres;

using AssociationRegistry.EventStore;
using AssociationRegistry.Framework;
using AssociationRegistry.Vereniging;
using NodaTime;

public class TeOntkoppelenLocatiesHandler
{
    private readonly IVerenigingsRepository _repository;

    public TeOntkoppelenLocatiesHandler(IVerenigingsRepository repository)
    {
        _repository = repository;
    }

    public async Task Handle(TeOntkoppelenLocatiesMessage message, CancellationToken cancellationToken)
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

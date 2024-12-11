namespace AssociationRegistry.Acties.AanvaardDubbel;

using AssociationRegistry.EventStore;
using AssociationRegistry.Framework;
using AssociationRegistry.Vereniging;
using NodaTime;

public class AanvaardDubbeleVerenigingCommandHandler(IVerenigingsRepository repository)
{
    public async Task Handle(AanvaardDubbeleVerenigingCommand command, CancellationToken cancellationToken)
    {
        var vereniging = await repository.Load<Vereniging>(command.VCode);

        vereniging.AanvaardDubbeleVereniging(command.VCodeDubbeleVereniging);

        await repository.Save(
            vereniging,
            new CommandMetadata(EventStore.DigitaalVlaanderenOvoNumber,
                                SystemClock.Instance.GetCurrentInstant(),
                                Guid.NewGuid()),
            cancellationToken);
    }
}

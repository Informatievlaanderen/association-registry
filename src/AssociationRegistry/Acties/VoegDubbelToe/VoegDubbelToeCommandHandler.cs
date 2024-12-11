namespace AssociationRegistry.Acties.VoegDubbelToe;

using EventStore;
using Framework;
using Vereniging;
using NodaTime;

public class VoegDubbelToeCommandHandler(IVerenigingsRepository repository)
{
    public async Task Handle(VoegDubbelToeCommand command, CancellationToken cancellationToken)
    {
        var vereniging = await repository.Load<Vereniging>(command.VCode);

        vereniging.VoegDubbeleVerenigingToe(command.VCodeDubbeleVereniging);

        await repository.Save(
            vereniging,
            new CommandMetadata(EventStore.DigitaalVlaanderenOvoNumber,
                                SystemClock.Instance.GetCurrentInstant(),
                                Guid.NewGuid()),
            cancellationToken);
    }
}

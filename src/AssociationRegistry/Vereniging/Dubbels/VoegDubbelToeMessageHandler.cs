namespace AssociationRegistry.Vereniging.Dubbels;

using EventStore;
using Framework;
using NodaTime;

public class VoegDubbelToeMessageHandler(IVerenigingsRepository repository)
{
    public async Task Handle(VoegDubbelToeMessage message, CancellationToken cancellationToken)
    {
        var vereniging = await repository.Load<Vereniging>(message.VCode);

        vereniging.VoegDubbeleVerenigingToe(message.VCodeDubbeleVereniging);

        await repository.Save(
            vereniging,
            new CommandMetadata(EventStore.DigitaalVlaanderenOvoNumber,
                                SystemClock.Instance.GetCurrentInstant(),
                                Guid.NewGuid()),
            cancellationToken);
    }
}

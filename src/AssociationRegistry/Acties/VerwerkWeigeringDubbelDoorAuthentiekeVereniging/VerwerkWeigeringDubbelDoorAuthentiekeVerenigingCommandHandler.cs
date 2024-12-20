namespace AssociationRegistry.Acties.VerwerkWeigeringDubbelDoorAuthentiekeVereniging;

using EventStore;
using Framework;
using NodaTime;
using Vereniging;

public class VerwerkWeigeringDubbelDoorAuthentiekeVerenigingCommandHandler(IVerenigingsRepository repository)
{
    public async Task Handle(VerwerkWeigeringDubbelDoorAuthentiekeVerenigingCommand command, CancellationToken cancellationToken)
    {
        var vereniging = await repository.Load<VerenigingOfAnyKind>(command.VCode);

        vereniging.VerwerkWeigeringDubbelDoorAuthentiekeVereniging(command.VCode);

        await repository.Save(
            vereniging,
            new CommandMetadata(EventStore.DigitaalVlaanderenOvoNumber,
                                SystemClock.Instance.GetCurrentInstant(),
                                Guid.NewGuid()),
            cancellationToken);
    }
}

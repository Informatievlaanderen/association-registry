namespace AssociationRegistry.Acties.AanvaardCorrectieDubbel;

using AssociationRegistry.Vereniging;
using Wolverine;

public class AanvaardCorrectieDubbeleVerenigingCommandHandler(
    IVerenigingsRepository repository)
{
    public async Task Handle(AanvaardCorrectieDubbeleVerenigingCommand command, CancellationToken cancellationToken)
    {
        // try
        // {
        //     var vereniging = await repository.Load<VerenigingOfAnyKind>(command.VCode);
        //
        //     vereniging.AanvaardDubbeleVereniging(command.VCodeDubbeleVereniging);
        //
        //     await repository.Save(
        //         vereniging,
        //         new CommandMetadata(EventStore.DigitaalVlaanderenOvoNumber,
        //                             SystemClock.Instance.GetCurrentInstant(),
        //                             Guid.NewGuid()),
        //         cancellationToken);
        // }
        // catch (Exception)
        // {
        //     await bus.SendAsync(new VerwerkWeigeringDubbelDoorAuthentiekeVerenigingMessage(command.VCodeDubbeleVereniging, command.VCode));
        //     throw;
        // }
    }
}

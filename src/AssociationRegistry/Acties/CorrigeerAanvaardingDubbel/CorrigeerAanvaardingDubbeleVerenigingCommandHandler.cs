namespace AssociationRegistry.Acties.CorrigeerAanvaardingDubbel;

using AanvaardDubbel;
using AssociationRegistry.EventStore;
using AssociationRegistry.Framework;
using AssociationRegistry.Messages;
using AssociationRegistry.Vereniging;
using NodaTime;
using Wolverine;

public class CorrigeerAanvaardingDubbeleVerenigingCommandHandler(
    IVerenigingsRepository repository,
    IMessageBus bus)
{
    public async Task Handle(CorrigeerAanvaardingDubbeleVerenigingCommand command, CancellationToken cancellationToken)
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

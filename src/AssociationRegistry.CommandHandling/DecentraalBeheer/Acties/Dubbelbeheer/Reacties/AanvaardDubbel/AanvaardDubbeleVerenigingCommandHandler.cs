namespace AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Dubbelbeheer.Reacties.AanvaardDubbel;

using System.Threading;
using System.Threading.Tasks;
using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.Framework;
using Be.Vlaanderen.Basisregisters.AggregateSource;
using MartenDb.Store;
using VerwerkWeigeringDubbelDoorAuthentiekeVereniging;
using Wolverine;

public class AanvaardDubbeleVerenigingCommandHandler(IAggregateSession aggregateSession, IMessageBus bus)
{
    public async Task Handle(AanvaardDubbeleVerenigingCommand command, CancellationToken cancellationToken)
    {
        try
        {
            var metadata = CommandMetadata.ForDigitaalVlaanderenProcess;

            var vereniging = await aggregateSession.Load<VerenigingOfAnyKind>(command.VCode, metadata);

            vereniging.AanvaardDubbeleVereniging(command.VCodeDubbeleVereniging);

            await aggregateSession.Save(vereniging, metadata, cancellationToken);
        }
        catch (AggregateNotFoundException)
        {
            await bus.SendAsync(
                new VerwerkWeigeringDubbelDoorAuthentiekeVerenigingMessage(
                    command.VCodeDubbeleVereniging,
                    command.VCode
                )
            );
        }
        catch (DomainException)
        {
            await bus.SendAsync(
                new VerwerkWeigeringDubbelDoorAuthentiekeVerenigingMessage(
                    command.VCodeDubbeleVereniging,
                    command.VCode
                )
            );
        }
    }
}

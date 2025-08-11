namespace AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Dubbelbeheer.Reacties.AanvaardDubbel;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.Framework;
using Be.Vlaanderen.Basisregisters.AggregateSource;
using System.Threading;
using System.Threading.Tasks;
using VerwerkWeigeringDubbelDoorAuthentiekeVereniging;
using Wolverine;

public class AanvaardDubbeleVerenigingCommandHandler(
    IVerenigingsRepository repository,
    IMessageBus bus)
{
    public async Task Handle(AanvaardDubbeleVerenigingCommand command, CancellationToken cancellationToken)
    {
        try
        {
            var metadata = CommandMetadata.ForDigitaalVlaanderenProcess;

            var vereniging = await repository.Load<VerenigingOfAnyKind>(command.VCode, metadata);

            vereniging.AanvaardDubbeleVereniging(command.VCodeDubbeleVereniging);

            await repository.Save(
                vereniging,
                metadata,
                cancellationToken);
        }
        catch (AggregateNotFoundException)
        {
            await bus.SendAsync(new VerwerkWeigeringDubbelDoorAuthentiekeVerenigingMessage(command.VCodeDubbeleVereniging, command.VCode));
        }
        catch (DomainException)
        {
            await bus.SendAsync(new VerwerkWeigeringDubbelDoorAuthentiekeVerenigingMessage(command.VCodeDubbeleVereniging, command.VCode));
        }
    }
}

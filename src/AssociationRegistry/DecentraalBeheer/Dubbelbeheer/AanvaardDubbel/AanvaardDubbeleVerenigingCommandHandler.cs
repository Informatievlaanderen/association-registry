namespace AssociationRegistry.DecentraalBeheer.Dubbelbeheer.AanvaardDubbel;

using EventStore;
using Framework;
using Messages;
using Vereniging;
using Be.Vlaanderen.Basisregisters.AggregateSource;
using NodaTime;
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

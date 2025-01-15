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
            var vereniging = await repository.Load<VerenigingOfAnyKind>(command.VCode);

            vereniging.AanvaardDubbeleVereniging(command.VCodeDubbeleVereniging);

            await repository.Save(
                vereniging,
                new CommandMetadata(EventStore.DigitaalVlaanderenOvoNumber,
                                    SystemClock.Instance.GetCurrentInstant(),
                                    Guid.NewGuid()),
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

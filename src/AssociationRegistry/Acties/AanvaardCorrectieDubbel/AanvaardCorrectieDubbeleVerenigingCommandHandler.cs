namespace AssociationRegistry.Acties.AanvaardCorrectieDubbel;

using EventStore;
using Framework;
using Vereniging;
using NodaTime;

public class AanvaardCorrectieDubbeleVerenigingCommandHandler(
    IVerenigingsRepository repository)
{
    public async Task Handle(AanvaardCorrectieDubbeleVerenigingCommand command, CancellationToken cancellationToken)
    {

            var vereniging = await repository.Load<VerenigingOfAnyKind>(command.VCode);

            vereniging.AanvaardCorrectieDubbeleVereniging(command.VCodeDubbeleVereniging);

            await repository.Save(
                vereniging,
                new CommandMetadata(EventStore.DigitaalVlaanderenOvoNumber,
                                    SystemClock.Instance.GetCurrentInstant(),
                                    Guid.NewGuid()),
                cancellationToken);
    }
}

namespace AssociationRegistry.Acties.CorrigeerMarkeerAlsDubbel;

using EventStore;
using Framework;
using NodaTime;
using Vereniging;

public class CorrigeerMarkeerAlsDubbeleVerenigingCommandHandler(IVerenigingsRepository repository)
{
    public async Task Handle(CorrigeerMarkeerAlsDubbeleVerenigingCommand command, CancellationToken cancellationToken)
    {
        var vereniging = await repository.Load<VerenigingOfAnyKind>(command.VCode);

        vereniging.CorrigeerMarkeerAlsDubbeleVereniging(command.VCode);

        await repository.Save(
            vereniging,
            new CommandMetadata(EventStore.DigitaalVlaanderenOvoNumber,
                                SystemClock.Instance.GetCurrentInstant(),
                                Guid.NewGuid()),
            cancellationToken);
    }
}

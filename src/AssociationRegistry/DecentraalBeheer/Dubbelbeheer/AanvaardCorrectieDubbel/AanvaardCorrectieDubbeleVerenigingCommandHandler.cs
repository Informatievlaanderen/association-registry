namespace AssociationRegistry.DecentraalBeheer.Dubbelbeheer.AanvaardCorrectieDubbel;

using AssociationRegistry.Framework;
using AssociationRegistry.Vereniging;

public class AanvaardCorrectieDubbeleVerenigingCommandHandler(
    IVerenigingsRepository repository)
{
    public async Task Handle(AanvaardCorrectieDubbeleVerenigingCommand command, CancellationToken cancellationToken)
    {
        var metadata = CommandMetadata.ForDigitaalVlaanderenProcess;
        var vereniging = await repository.Load<VerenigingOfAnyKind>(command.VCode, metadata);

        vereniging.AanvaardCorrectieDubbeleVereniging(command.VCodeDubbeleVereniging);

        await repository.Save(
            vereniging,
            metadata,
            cancellationToken);
    }
}

namespace AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Dubbelbeheer.Reacties.AanvaardCorrectieDubbel;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.Framework;
using System.Threading;
using System.Threading.Tasks;

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

namespace AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Dubbelbeheer.Reacties.AanvaardCorrectieDubbel;

using System.Threading;
using System.Threading.Tasks;
using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.Framework;
using MartenDb.Store;

public class AanvaardCorrectieDubbeleVerenigingCommandHandler(IAggregateSession aggregateSession)
{
    public async Task Handle(AanvaardCorrectieDubbeleVerenigingCommand command, CancellationToken cancellationToken)
    {
        var metadata = CommandMetadata.ForDigitaalVlaanderenProcess;
        var vereniging = await aggregateSession.Load<VerenigingOfAnyKind>(command.VCode, metadata);

        vereniging.AanvaardCorrectieDubbeleVereniging(command.VCodeDubbeleVereniging);

        await aggregateSession.Save(vereniging, metadata, cancellationToken);
    }
}

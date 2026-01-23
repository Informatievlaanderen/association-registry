namespace AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Subtype;

using System.Threading;
using System.Threading.Tasks;
using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.DecentraalBeheer.Vereniging.Exceptions;
using AssociationRegistry.Framework;
using MartenDb.Store;

public class VerfijnSubtypeNaarSubverenigingCommandHandler
{
    private readonly IAggregateSession _aggregateSession;

    public VerfijnSubtypeNaarSubverenigingCommandHandler(IAggregateSession aggregateSession)
    {
        _aggregateSession = aggregateSession;
    }

    public async Task<CommandResult> Handle(
        CommandEnvelope<VerfijnSubtypeNaarSubverenigingCommand> envelope,
        CancellationToken cancellationToken = default
    )
    {
        var vereniging = await _aggregateSession.Load<Vereniging>(
            VCode.Create(envelope.Command.VCode),
            envelope.Metadata
        );

        if (envelope.Command.SubverenigingVan.AndereVereniging is not null)
        {
            try
            {
                var verenigingMetRechtspersoonlijkheid =
                    await _aggregateSession.Load<VerenigingMetRechtspersoonlijkheid>(
                        VCode.Create(envelope.Command.SubverenigingVan.AndereVereniging),
                        envelope.Metadata
                    );

                envelope.Command.SubverenigingVan.AndereVerenigingNaam = verenigingMetRechtspersoonlijkheid.Naam;
            }
            catch (VerenigingIsVerwijderd)
            {
                throw new AndereVerenigingIsVerwijderd();
            }
            catch (ActieIsNietToegestaanVoorVerenigingstype e)
            {
                throw new ActieIsNietToegestaanVoorAndereVerenigingVerenigingstype();
            }
        }

        vereniging.VerfijnNaarSubvereniging(envelope.Command.SubverenigingVan);

        var result = await _aggregateSession.Save(vereniging, envelope.Metadata, cancellationToken);

        return CommandResult.Create(VCode.Create(envelope.Command.VCode), result);
    }
}

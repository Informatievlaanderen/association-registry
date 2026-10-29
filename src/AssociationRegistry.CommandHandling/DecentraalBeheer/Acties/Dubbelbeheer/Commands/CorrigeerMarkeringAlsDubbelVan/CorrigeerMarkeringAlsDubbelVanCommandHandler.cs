namespace AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Dubbelbeheer.Commands.CorrigeerMarkeringAlsDubbelVan;

using System.Threading;
using System.Threading.Tasks;
using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.Framework;
using MartenDb.Store;

/// <summary>
///     Corrigeert de markering van de dubbele vereniging en laat de authentieke vereniging de correctie
///     aanvaarden in één transactie, zodat beide verenigingen altijd samen gewijzigd worden.
///     Werd één van beide intussen gewijzigd, dan wordt alles opnieuw geprobeerd.
/// </summary>
public class CorrigeerMarkeringAlsDubbelVanCommandHandler
{
    private readonly IAggregateSession _aggregateSession;

    public CorrigeerMarkeringAlsDubbelVanCommandHandler(IAggregateSession aggregateSession)
    {
        _aggregateSession = aggregateSession;
    }

    public Task<CommandResult> Handle(
        CommandEnvelope<CorrigeerMarkeringAlsDubbelVanCommand> envelope,
        CancellationToken cancellationToken = default
    ) =>
        DubbelbeheerRetryPolicy.OnUnexpectedAggregateVersion.ExecuteAsync(
            _ => CorrigeerMarkeringAlsDubbelVan(envelope, cancellationToken),
            cancellationToken
        );

    private async Task<CommandResult> CorrigeerMarkeringAlsDubbelVan(
        CommandEnvelope<CorrigeerMarkeringAlsDubbelVanCommand> envelope,
        CancellationToken cancellationToken
    )
    {
        var vereniging = await _aggregateSession.Load<Vereniging>(
            envelope.Command.VCode,
            envelope.Metadata,
            allowDubbeleVereniging: true
        );

        var vCodeAuthentiekeVereniging = vereniging.CorrigeerMarkeringAlsDubbelVan();

        // De authentieke vereniging kan intussen zelf als dubbel gemarkeerd zijn (bv. A -> B, daarna B -> C).
        // Ook dan moet de correctie van A verwerkt worden.
        // De verwachte versie hoort bij de dubbele vereniging, niet bij de authentieke.
        var authentiekeVereniging = await _aggregateSession.Load<VerenigingOfAnyKind>(
            VCode.Create(vCodeAuthentiekeVereniging),
            envelope.Metadata with
            {
                ExpectedVersion = null,
            },
            allowDubbeleVereniging: true
        );

        authentiekeVereniging.AanvaardCorrectieDubbeleVereniging(envelope.Command.VCode);

        var results = await _aggregateSession.SaveTransactional(
            [vereniging, authentiekeVereniging],
            envelope.Metadata,
            cancellationToken
        );

        return CommandResult.Create(envelope.Command.VCode, results[0]);
    }
}

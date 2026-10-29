namespace AssociationRegistry.Scheduled.Host.DubbelBeheer;

using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Dubbelbeheer.Commands.MarkeerAlsDubbelVan;
using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.Framework;
using Marten;
using Microsoft.Extensions.Logging;
using Schema;
using Wolverine;

/// <summary>
///     Markeert de dubbele vereniging als dubbel van de authentieke vereniging. De authentieke vereniging aanvaardt
///     in dezelfde transactie, zodat een volgende rij de ketting meteen kan verderzetten (A -> B, daarna B -> C).
///     Werd de rij al verwerkt (bv. een hervatte run die stopte voor de status bewaard werd), dan is ze klaar.
/// </summary>
public class DubbelImportLineProcessor(
    IMessageBus messageBus,
    IDocumentStore store,
    ILogger<DubbelImportLineProcessor> logger
) : IDubbelImportLineProcessor
{
    public async Task<DubbelImportLineResult> Process(DubbelImportDocument line, CancellationToken cancellationToken)
    {
        try
        {
            var dubbeleVCode = VCode.Create(line.DubbeleVCode);
            var authentiekeVCode = VCode.Create(line.AuthentiekeVCode);

            if (await IsDubbelVan(dubbeleVCode, authentiekeVCode, cancellationToken))
                return DubbelImportLineResult.Processed;

            await messageBus.InvokeAsync<CommandResult>(
                new CommandEnvelope<MarkeerAlsDubbelVanCommand>(
                    new MarkeerAlsDubbelVanCommand(dubbeleVCode, authentiekeVCode),
                    CommandMetadata.ForDigitaalVlaanderenProcess
                ),
                cancellationToken
            );

            return DubbelImportLineResult.Processed;
        }
        catch (Exception e) when (e is not OperationCanceledException)
        {
            logger.LogWarning(
                e,
                "Rij {LineNumber}: {DubbeleVCode} kon niet als dubbel van {AuthentiekeVCode} gemarkeerd worden: {Reason}",
                line.LineNumber,
                line.DubbeleVCode,
                line.AuthentiekeVCode,
                e.Message
            );

            return DubbelImportLineResult.Failed(e.Message);
        }
    }

    /// <summary>Leest rechtstreeks uit de event store, zodat er niet op een projectie gewacht moet worden.</summary>
    private async Task<bool> IsDubbelVan(
        VCode dubbeleVCode,
        VCode authentiekeVCode,
        CancellationToken cancellationToken
    )
    {
        await using var session = store.QuerySession();

        var dubbele = await session.Events.AggregateStreamAsync<VerenigingState>(
            dubbeleVCode,
            token: cancellationToken
        );

        return dubbele?.VerenigingStatus is VerenigingStatus.StatusDubbel { VCodeAuthentiekeVereniging: var vCode }
            && vCode.Equals(authentiekeVCode);
    }
}

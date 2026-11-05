namespace AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Dubbelbeheer.Commands.MarkeerAlsDubbelVan;

using System.Threading;
using System.Threading.Tasks;
using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.DecentraalBeheer.Vereniging.Exceptions;
using AssociationRegistry.DecentraalBeheer.Vereniging.Geotags;
using AssociationRegistry.DecentraalBeheer.Vereniging.Samenvoeging;
using Framework;
using MartenDb.Store;

/// <summary>
///     Markeert de dubbele vereniging, laat de authentieke vereniging ze aanvaarden en voegt de gegevens van de
///     dubbele samen met de authentieke, in één transactie, zodat beide verenigingen altijd samen gewijzigd worden.
///     Werd één van beide intussen gewijzigd, dan wordt alles opnieuw geprobeerd.
/// </summary>
public class MarkeerAlsDubbelVanCommandHandler
{
    private readonly IAggregateSession _aggregateSession;
    private readonly IVerenigingStateQueryService _queryService;
    private readonly IGeotagsService _geotagsService;

    public MarkeerAlsDubbelVanCommandHandler(
        IAggregateSession aggregateSession,
        IVerenigingStateQueryService queryService,
        IGeotagsService geotagsService
    )
    {
        _aggregateSession = aggregateSession;
        _queryService = queryService;
        _geotagsService = geotagsService;
    }

    public Task<CommandResult> Handle(
        CommandEnvelope<MarkeerAlsDubbelVanCommand> message,
        CancellationToken cancellationToken = default
    ) =>
        DubbelbeheerRetryPolicy.OnUnexpectedAggregateVersion.ExecuteAsync(
            _ => MarkeerAlsDubbelVan(message, cancellationToken),
            cancellationToken
        );

    private async Task<CommandResult> MarkeerAlsDubbelVan(
        CommandEnvelope<MarkeerAlsDubbelVanCommand> message,
        CancellationToken cancellationToken
    )
    {
        var vereniging = await _aggregateSession.Load<Vereniging>(
            vCode: message.Command.VCode,
            metadata: message.Metadata
        );

        await ThrowIfInvalidAuthentiekeVereniging(message);

        // De verwachte versie hoort bij de dubbele vereniging, niet bij de authentieke.
        var authentiekeVereniging = await _aggregateSession.Load<VerenigingOfAnyKind>(
            vCode: message.Command.VCodeAuthentiekeVereniging,
            metadata: message.Metadata with
            {
                ExpectedVersion = null,
            }
        );

        vereniging.MarkeerAlsDubbelVan(message.Command.VCodeAuthentiekeVereniging);
        authentiekeVereniging.AanvaardDubbeleVereniging(message.Command.VCode);
        authentiekeVereniging.VoegDubbeleVerenigingSamen(
            vereniging,
            await StatusVanVerenigingenVanLidmaatschappen(vereniging)
        );

        // De samenvoeging kan locaties en werkingsgebieden toevoegen.
        await authentiekeVereniging.HerberekenGeotags(_geotagsService);

        var results = await _aggregateSession.SaveTransactional(
            verenigingen: [vereniging, authentiekeVereniging],
            metadata: message.Metadata,
            cancellationToken: cancellationToken
        );

        return CommandResult.Create(vCode: message.Command.VCode, streamActionResult: results[0]);
    }

    /// <summary>
    ///     Een lidmaatschap bij een verwijderde, dubbele of gestopte vereniging wordt niet overgenomen.
    /// </summary>
    private async Task<IReadOnlyDictionary<VCode, AndereVerenigingStatus>> StatusVanVerenigingenVanLidmaatschappen(
        Vereniging dubbeleVereniging
    )
    {
        var statussen = new Dictionary<VCode, AndereVerenigingStatus>();

        foreach (var andereVereniging in dubbeleVereniging.VerenigingenVanLidmaatschappen)
            statussen[andereVereniging] = await StatusVan(andereVereniging);

        return statussen;
    }

    private async Task<AndereVerenigingStatus> StatusVan(VCode vCode)
    {
        if (await _queryService.IsVerwijderd(vCode))
            return AndereVerenigingStatus.Verwijderd;

        if (await _queryService.IsDubbel(vCode))
            return AndereVerenigingStatus.Dubbel;

        if (await _queryService.IsGestopt(vCode))
            return AndereVerenigingStatus.Gestopt;

        return AndereVerenigingStatus.Actief;
    }

    private async Task ThrowIfInvalidAuthentiekeVereniging(CommandEnvelope<MarkeerAlsDubbelVanCommand> message)
    {
        if (!await _queryService.Exists(message.Command.VCodeAuthentiekeVereniging))
            throw new VerenigingKanGeenDubbelWordenVanEenNietBestaandeVereniging();

        if (await _queryService.IsVerwijderd(message.Command.VCodeAuthentiekeVereniging))
            throw new VerenigingKanGeenDubbelWordenVanVerwijderdeVereniging();

        if (await _queryService.IsDubbel(message.Command.VCodeAuthentiekeVereniging))
            throw new VerenigingKanGeenDubbelWordenVanEenVerenigingReedsGemarkeerdAlsDubbel();
    }
}

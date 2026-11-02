namespace AssociationRegistry.Test.E2E.Scenarios.Givens.FeitelijkeVereniging;

using AssociationRegistry.Framework;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.DubbelMerge;
using AutoFixture;
using DecentraalBeheer.Vereniging;
using Events;
using EventStore;
using MartenDb.Store;
using Vereniging;

public class MultipleWerdGeregistreerdScenario : Framework.TestClasses.IScenario
{
    public FeitelijkeVerenigingWerdGeregistreerd FeitelijkeVerenigingWerdGeregistreerd { get; set; }
    public FeitelijkeVerenigingWerdGeregistreerd AndereFeitelijkeVerenigingWerdGeregistreerd { get; set; }
    private CommandMetadata Metadata;
    private readonly TestVerenigingsgegevens? _gegevens;
    private readonly TestVerenigingsgegevens? _andereGegevens;

    /// <param name="gegevens">Vaste gegevens voor FeitelijkeVerenigingWerdGeregistreerd, anders willekeurig.</param>
    /// <param name="andereGegevens">Vaste gegevens voor AndereFeitelijkeVerenigingWerdGeregistreerd, anders willekeurig.</param>
    public MultipleWerdGeregistreerdScenario(
        TestVerenigingsgegevens? gegevens = null,
        TestVerenigingsgegevens? andereGegevens = null
    )
    {
        _gegevens = gegevens;
        _andereGegevens = andereGegevens;
    }

    public async Task<KeyValuePair<string, IEvent[]>[]> GivenEvents(IVCodeService service)
    {
        var fixture = new Fixture().CustomizeAdminApi();

        FeitelijkeVerenigingWerdGeregistreerd = WithGegevens(
            fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>() with
            {
                VCode = await service.GetNext(),
            },
            _gegevens
        );
        AndereFeitelijkeVerenigingWerdGeregistreerd = WithGegevens(
            fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>() with
            {
                VCode = await service.GetNext(),
                Vertegenwoordigers = FeitelijkeVerenigingWerdGeregistreerd.Vertegenwoordigers,
            },
            _andereGegevens
        );

        Metadata = fixture.Create<CommandMetadata>() with { ExpectedVersion = null };

        return
        [
            new(
                FeitelijkeVerenigingWerdGeregistreerd.VCode,
                EventsFor(FeitelijkeVerenigingWerdGeregistreerd, _gegevens)
            ),
            new(
                AndereFeitelijkeVerenigingWerdGeregistreerd.VCode,
                EventsFor(AndereFeitelijkeVerenigingWerdGeregistreerd, _andereGegevens)
            ),
        ];
    }

    private static FeitelijkeVerenigingWerdGeregistreerd WithGegevens(
        FeitelijkeVerenigingWerdGeregistreerd @event,
        TestVerenigingsgegevens? gegevens
    ) =>
        gegevens is null
            ? @event
            : @event
                .WithBasisgegevens(gegevens.Basisgegevens)
                .WithHoofdactiviteiten(gegevens.Hoofdactiviteiten)
                .WithContactgegevens(gegevens.Contactgegevens);

    private static IEvent[] EventsFor(
        FeitelijkeVerenigingWerdGeregistreerd @event,
        TestVerenigingsgegevens? gegevens
    ) =>
        [
            @event,
            new FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheid(@event.VCode),
            .. gegevens is { Werkingsgebieden.Length: > 0 }
                ? [new WerkingsgebiedenWerdenBepaald(@event.VCode, gegevens.Werkingsgebieden)]
                : Array.Empty<IEvent>(),
        ];

    public StreamActionResult Result { get; set; } = null!;

    public CommandMetadata GetCommandMetadata() => Metadata;
}

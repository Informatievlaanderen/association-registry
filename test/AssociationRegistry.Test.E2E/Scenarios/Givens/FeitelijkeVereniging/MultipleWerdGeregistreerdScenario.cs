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
    private readonly TestBasisgegevens? _basisgegevens;
    private readonly TestBasisgegevens? _andereBasisgegevens;

    /// <param name="basisgegevens">Vaste basisgegevens voor FeitelijkeVerenigingWerdGeregistreerd, anders willekeurig.</param>
    /// <param name="andereBasisgegevens">Vaste basisgegevens voor AndereFeitelijkeVerenigingWerdGeregistreerd, anders willekeurig.</param>
    public MultipleWerdGeregistreerdScenario(
        TestBasisgegevens? basisgegevens = null,
        TestBasisgegevens? andereBasisgegevens = null
    )
    {
        _basisgegevens = basisgegevens;
        _andereBasisgegevens = andereBasisgegevens;
    }

    public async Task<KeyValuePair<string, IEvent[]>[]> GivenEvents(IVCodeService service)
    {
        var fixture = new Fixture().CustomizeAdminApi();

        FeitelijkeVerenigingWerdGeregistreerd = fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>() with
        {
            VCode = await service.GetNext(),
        };
        AndereFeitelijkeVerenigingWerdGeregistreerd = fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>() with
        {
            VCode = await service.GetNext(),
            Vertegenwoordigers = FeitelijkeVerenigingWerdGeregistreerd.Vertegenwoordigers,
        };

        if (_basisgegevens is not null)
            FeitelijkeVerenigingWerdGeregistreerd = FeitelijkeVerenigingWerdGeregistreerd.WithBasisgegevens(
                _basisgegevens
            );

        if (_andereBasisgegevens is not null)
            AndereFeitelijkeVerenigingWerdGeregistreerd = AndereFeitelijkeVerenigingWerdGeregistreerd.WithBasisgegevens(
                _andereBasisgegevens
            );

        Metadata = fixture.Create<CommandMetadata>() with { ExpectedVersion = null };

        return
        [
            new(
                FeitelijkeVerenigingWerdGeregistreerd.VCode,
                [
                    FeitelijkeVerenigingWerdGeregistreerd,
                    new FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheid(
                        FeitelijkeVerenigingWerdGeregistreerd.VCode
                    ),
                ]
            ),
            new(
                AndereFeitelijkeVerenigingWerdGeregistreerd.VCode,
                [
                    AndereFeitelijkeVerenigingWerdGeregistreerd,
                    new FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheid(
                        AndereFeitelijkeVerenigingWerdGeregistreerd.VCode
                    ),
                ]
            ),
        ];
    }

    public StreamActionResult Result { get; set; } = null!;

    public CommandMetadata GetCommandMetadata() => Metadata;
}

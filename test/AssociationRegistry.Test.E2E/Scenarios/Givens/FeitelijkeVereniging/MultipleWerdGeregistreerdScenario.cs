namespace AssociationRegistry.Test.E2E.Scenarios.Givens.FeitelijkeVereniging;

using Admin.Schema.Persoonsgegevens;
using AssociationRegistry.Framework;
using AutoFixture;
using Common.AutoFixture;
using DecentraalBeheer.Vereniging;
using Events;
using Events.Enriched;
using EventStore;
using Framework.Mappers;
using Framework.TestClasses;

public class MultipleWerdGeregistreerdScenario : IScenario
{
    public FeitelijkeVerenigingWerdGeristreerdMetPersoonsgegevens FeitelijkeVerenigingWerdGeristreerdMetPersoonsgegevens { get; set; }
    public FeitelijkeVerenigingWerdGeregistreerd FeitelijkeVerenigingWerdGeregistreerd { get; set; }
    public FeitelijkeVerenigingWerdGeristreerdMetPersoonsgegevens AndereFeitelijkeVerenigingWerdGeristreerdMetPersoonsgegevens { get; set; }
    public FeitelijkeVerenigingWerdGeregistreerd AndereFeitelijkeVerenigingWerdGeregistreerd { get; set; }

    private CommandMetadata Metadata;
    private VertegenwoordigerPersoonsgegevensDocument[] _vertegenwoordigerPersoonsgegevens;

    public MultipleWerdGeregistreerdScenario()
    {
        var fixture = new Fixture().CustomizeAdminApi();

        FeitelijkeVerenigingWerdGeristreerdMetPersoonsgegevens = fixture.Create<FeitelijkeVerenigingWerdGeristreerdMetPersoonsgegevens>() with
        {
            VCode = fixture.Create<VCode>(),
        };
        AndereFeitelijkeVerenigingWerdGeristreerdMetPersoonsgegevens = fixture.Create<FeitelijkeVerenigingWerdGeristreerdMetPersoonsgegevens>() with
        {
            VCode = fixture.Create<VCode>(),
            Vertegenwoordigers = FeitelijkeVerenigingWerdGeristreerdMetPersoonsgegevens.Vertegenwoordigers,
        };

        var (feitelijkeVerenigingWerdGeregistreerd, persoonsgegevensDocuments) = FeitelijkeVerenigingWerdGeristreerdMetPersoonsgegevens.MapDomainWithPersoonsgegevens();
        var (andereFeitelijkeVerenigingWerdGeregistreerd, andereVerenigingvertegenwoordigerPersoonsgegevensDocuments) = AndereFeitelijkeVerenigingWerdGeristreerdMetPersoonsgegevens.MapDomainWithPersoonsgegevens();
        FeitelijkeVerenigingWerdGeregistreerd = feitelijkeVerenigingWerdGeregistreerd;
        AndereFeitelijkeVerenigingWerdGeregistreerd = andereFeitelijkeVerenigingWerdGeregistreerd;
        _vertegenwoordigerPersoonsgegevens = persoonsgegevensDocuments
                                            .Concat(andereVerenigingvertegenwoordigerPersoonsgegevensDocuments)
                                            .ToArray();

        Metadata = fixture.Create<CommandMetadata>() with { ExpectedVersion = null };
    }

    public async Task<KeyValuePair<string, IEvent[]>[]> GivenEvents(IVCodeService service)
    {


        return
        [
            new(FeitelijkeVerenigingWerdGeregistreerd.VCode, [FeitelijkeVerenigingWerdGeregistreerd, new FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheid(FeitelijkeVerenigingWerdGeregistreerd.VCode)]),
            new(AndereFeitelijkeVerenigingWerdGeregistreerd.VCode, [AndereFeitelijkeVerenigingWerdGeregistreerd, new FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheid(AndereFeitelijkeVerenigingWerdGeregistreerd.VCode)]),
        ];
    }

    public VertegenwoordigerPersoonsgegevensDocument[] GivenVertegenwoordigerPersoonsgegevens()
        => _vertegenwoordigerPersoonsgegevens;

    public StreamActionResult Result { get; set; } = null!;

    public CommandMetadata GetCommandMetadata()
        => Metadata;
}

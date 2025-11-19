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
    public FeitelijkeVerenigingWerdGeregistreerd FeitelijkeVerenigingWerdGeregistreerd { get; set; }
    public FeitelijkeVerenigingWerdGeregistreerd AndereFeitelijkeVerenigingWerdGeregistreerd { get; set; }

    private CommandMetadata Metadata;
    public VertegenwoordigerPersoonsgegevensDocument[] VertegenwoordigerPersoonsgegevens;
    public VertegenwoordigerPersoonsgegevensDocument[] AndereVertegenwoordigerPersoonsgegevens;

    public MultipleWerdGeregistreerdScenario()
    {
        var fixture = new Fixture().CustomizeAdminApi();

        Metadata = fixture.Create<CommandMetadata>() with { ExpectedVersion = null };
    }

    public async Task<KeyValuePair<string, IEvent[]>[]> GivenEvents(IVCodeService service)
    {
        var geregistreerdeVerenigingMetPersoongegevens = await EventMapper.CreateFeitelijkeGeregistreerdMetPersoonsgegevens(service);
        var geregistreerdEvent = geregistreerdeVerenigingMetPersoongegevens.GeregistreerdEvent;
        VertegenwoordigerPersoonsgegevens = geregistreerdeVerenigingMetPersoongegevens.PersoonsgegevensDocumenten;
        FeitelijkeVerenigingWerdGeregistreerd = geregistreerdEvent;

        var andereGeregistreerdeVerenigingMetPersoongegevens = await EventMapper.CreateFeitelijkeGeregistreerdMetPersoonsgegevens(service, geregistreerdeVerenigingMetPersoongegevens.PersoonsgegevensDocumenten[0].Insz);
        var andereGeregistreerdEvent = andereGeregistreerdeVerenigingMetPersoongegevens.GeregistreerdEvent;
        AndereVertegenwoordigerPersoonsgegevens = andereGeregistreerdeVerenigingMetPersoongegevens.PersoonsgegevensDocumenten;
        AndereFeitelijkeVerenigingWerdGeregistreerd = andereGeregistreerdEvent;

        return
        [
            new(FeitelijkeVerenigingWerdGeregistreerd.VCode, [FeitelijkeVerenigingWerdGeregistreerd, new FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheid(FeitelijkeVerenigingWerdGeregistreerd.VCode)]),
            new(AndereFeitelijkeVerenigingWerdGeregistreerd.VCode, [AndereFeitelijkeVerenigingWerdGeregistreerd, new FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheid(AndereFeitelijkeVerenigingWerdGeregistreerd.VCode)]),
        ];
    }

    public VertegenwoordigerPersoonsgegevensDocument[] GivenVertegenwoordigerPersoonsgegevens()
        => VertegenwoordigerPersoonsgegevens.Concat(AndereVertegenwoordigerPersoonsgegevens).ToArray();

    public StreamActionResult Result { get; set; } = null!;

    public CommandMetadata GetCommandMetadata()
        => Metadata;
}

namespace AssociationRegistry.Test.E2E.Scenarios.Givens.FeitelijkeVereniging;

using Admin.Schema.Persoonsgegevens;
using Events;
using EventStore;
using AssociationRegistry.Test.Common.AutoFixture;
using Vereniging;
using AutoFixture;
using DecentraalBeheer.Vereniging;
using MartenDb.Store;

public class LidmaatschapWerdToegevoegdScenario : Framework.TestClasses.IScenario
{
    public readonly MultipleWerdGeregistreerdScenario BaseScenario;
    public string NaamVereniging { get; set; }
    public LidmaatschapWerdToegevoegd LidmaatschapWerdToegevoegd { get; set; }

    public LidmaatschapWerdToegevoegdScenario(MultipleWerdGeregistreerdScenario baseScenario)
    {
        BaseScenario = baseScenario;
    }

    public async Task<KeyValuePair<string, IEvent[]>[]> GivenEvents(IVCodeService service)
    {
        var fixture = new Fixture().CustomizeAdminApi();

        var givenEvents = await BaseScenario.GivenEvents(service);
        NaamVereniging = BaseScenario.FeitelijkeVerenigingWerdGeristreerdMetPersoonsgegevens.Naam;

        LidmaatschapWerdToegevoegd = new LidmaatschapWerdToegevoegd(
            VCode: BaseScenario.FeitelijkeVerenigingWerdGeristreerdMetPersoonsgegevens.VCode,
            Lidmaatschap: fixture.Create<Registratiedata.Lidmaatschap>() with
            {
                AndereVereniging = BaseScenario.AndereFeitelijkeVerenigingWerdGeristreerdMetPersoonsgegevens.VCode,
                AndereVerenigingNaam = BaseScenario.AndereFeitelijkeVerenigingWerdGeristreerdMetPersoonsgegevens.Naam,
            });

        return givenEvents.Append(new KeyValuePair<string, IEvent[]>(LidmaatschapWerdToegevoegd.VCode, [LidmaatschapWerdToegevoegd]))
                          .ToArray();
    }

    public VertegenwoordigerPersoonsgegevensDocument[] GivenVertegenwoordigerPersoonsgegevens()
        => BaseScenario.GivenVertegenwoordigerPersoonsgegevens();

    public StreamActionResult Result { get; set; } = null!;
}

namespace AssociationRegistry.Test.E2E.Scenarios.Givens.FeitelijkeVereniging;

using Events;
using EventStore;
using AssociationRegistry.Test.Common.AutoFixture;
using Vereniging;
using AutoFixture;
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
        NaamVereniging = BaseScenario.FeitelijkeVerenigingWerdGeregistreerd.Naam;

        LidmaatschapWerdToegevoegd = new LidmaatschapWerdToegevoegd(
            VCode: BaseScenario.FeitelijkeVerenigingWerdGeregistreerd.VCode,
            Lidmaatschap: fixture.Create<Registratiedata.Lidmaatschap>() with
            {
                AndereVereniging = BaseScenario.AndereFeitelijkeVerenigingWerdGeregistreerd.VCode,
                AndereVerenigingNaam = BaseScenario.AndereFeitelijkeVerenigingWerdGeregistreerd.Naam,
            });

        return givenEvents.Append(new KeyValuePair<string, IEvent[]>(LidmaatschapWerdToegevoegd.VCode, [LidmaatschapWerdToegevoegd]))
                          .ToArray();
    }

    public StreamActionResult Result { get; set; } = null!;
}

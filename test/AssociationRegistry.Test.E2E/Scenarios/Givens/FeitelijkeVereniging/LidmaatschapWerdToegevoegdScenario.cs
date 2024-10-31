namespace AssociationRegistry.Test.E2E.Scenarios.Givens.FeitelijkeVereniging;

using Events;
using EventStore;
using AssociationRegistry.Framework;
using AssociationRegistry.Test.Common.AutoFixture;
using Vereniging;
using AutoFixture;

public class LidmaatschapWerdToegevoegdScenario : Framework.TestClasses.IScenario
{
    private readonly MultipleWerdGeregistreerdScenario _baseScenario;
    public LidmaatschapWerdToegevoegd LidmaatschapWerdToegevoegd { get; set; }

    public LidmaatschapWerdToegevoegdScenario(MultipleWerdGeregistreerdScenario baseScenario)
    {
        _baseScenario = baseScenario;
    }

    public async Task<KeyValuePair<string, IEvent[]>[]> GivenEvents(IVCodeService service)
    {
        var fixture = new Fixture().CustomizeAdminApi();

        var givenEvents = await _baseScenario.GivenEvents(service);

        LidmaatschapWerdToegevoegd = new LidmaatschapWerdToegevoegd(
            VCode: _baseScenario.FeitelijkeVerenigingWerdGeregistreerd.VCode,
            Lidmaatschap: fixture.Create<Registratiedata.Lidmaatschap>() with
            {
                AndereVereniging = _baseScenario.AndereFeitelijkeVerenigingWerdGeregistreerd.VCode,
            });

        return givenEvents.Append(new KeyValuePair<string, IEvent[]>(LidmaatschapWerdToegevoegd.VCode, [LidmaatschapWerdToegevoegd]))
                          .ToArray();
    }

    public StreamActionResult Result { get; set; } = null!;
}

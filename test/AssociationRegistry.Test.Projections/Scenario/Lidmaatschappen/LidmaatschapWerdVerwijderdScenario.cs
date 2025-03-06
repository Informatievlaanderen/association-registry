namespace AssociationRegistry.Test.Projections.Scenario.Lidmaatschappen;

using Events;
using System.Linq;

public class LidmaatschapWerdVerwijderdScenario : ScenarioBase
{
    private readonly LidmaatschapWerdToegevoegdScenario _werdToegevoegdScenario;
    public LidmaatschapWerdVerwijderd LidmaatschapWerdVerwijderd { get; }

    public LidmaatschapWerdVerwijderdScenario()
    {
        _werdToegevoegdScenario = new LidmaatschapWerdToegevoegdScenario();

        LidmaatschapWerdVerwijderd = new LidmaatschapWerdVerwijderd(
            Lidmaatschap: _werdToegevoegdScenario.LidmaatschapWerdToegevoegd.Lidmaatschap,
            VCode: _werdToegevoegdScenario.VerenigingWerdGeregistreerd.VCode);
    }

    public override string VCode => _werdToegevoegdScenario.VCode;

    public override EventsPerVCode[] Events => _werdToegevoegdScenario.Events.Union(
    [
        new EventsPerVCode(VCode, LidmaatschapWerdVerwijderd),
    ]).ToArray();
}

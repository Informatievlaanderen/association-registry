namespace AssociationRegistry.Test.Projections.Scenario.Lidmaatschappen;

using Events;
using AutoFixture;

public class LidmaatschapWerdGewijzigdScenario : ScenarioBase
{
    private readonly LidmaatschapWerdToegevoegdScenario _werdToegevoegdScenario = new();
    public LidmaatschapWerdGewijzigd LidmaatschapWerdGewijzigd { get; }

    public LidmaatschapWerdGewijzigdScenario()
    {
        LidmaatschapWerdGewijzigd = new LidmaatschapWerdGewijzigd(
            Lidmaatschap: AutoFixture.Create<Registratiedata.Lidmaatschap>() with
            {
                LidmaatschapId = _werdToegevoegdScenario.LidmaatschapWerdToegevoegd.Lidmaatschap.LidmaatschapId,
            },
            VCode: _werdToegevoegdScenario.VerenigingWerdGeregistreerd.VCode);
    }

    public override string VCode => _werdToegevoegdScenario.VCode;

    public override EventsPerVCode[] Events => _werdToegevoegdScenario.Events.Union(
    [
        new EventsPerVCode(VCode, LidmaatschapWerdGewijzigd),
    ]).ToArray();
}

namespace AssociationRegistry.Test.Projections.ScenarioClassFixtures;

using AutoFixture;
using Events;
using Framework;

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

    public override EventsPerVCode[] Events => _werdToegevoegdScenario.Events.Union(
    [
        new(_werdToegevoegdScenario.VerenigingWerdGeregistreerd.VCode, LidmaatschapWerdGewijzigd),
    ]).ToArray();
}

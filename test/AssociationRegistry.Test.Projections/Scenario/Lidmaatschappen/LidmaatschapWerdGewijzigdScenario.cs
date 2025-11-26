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
                LidmaatschapId = _werdToegevoegdScenario.LidmaatschapWerdToegevoegdFirst.Lidmaatschap.LidmaatschapId,
            },
            VCode: _werdToegevoegdScenario.VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode);
    }

    public override string AggregateId => _werdToegevoegdScenario.AggregateId;

    public override EventsPerVCode[] Events => _werdToegevoegdScenario.Events.Union(
    [
        new EventsPerVCode(AggregateId, LidmaatschapWerdGewijzigd),
    ]).ToArray();
}

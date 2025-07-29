namespace AssociationRegistry.Test.Projections.Scenario.Lidmaatschappen;

using Events;

public class LidmaatschapWerdVerwijderdScenario : ScenarioBase
{
    private readonly LidmaatschapWerdToegevoegdScenario _werdToegevoegdScenario;
    public LidmaatschapWerdVerwijderd LidmaatschapWerdVerwijderd { get; }

    public LidmaatschapWerdVerwijderdScenario()
    {
        _werdToegevoegdScenario = new LidmaatschapWerdToegevoegdScenario();

        LidmaatschapWerdVerwijderd = new LidmaatschapWerdVerwijderd(
            Lidmaatschap: _werdToegevoegdScenario.LidmaatschapWerdToegevoegdFirst.Lidmaatschap,
            VCode: _werdToegevoegdScenario.VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode);
    }

    public override string VCode => _werdToegevoegdScenario.VCode;

    public override EventsPerVCode[] Events => _werdToegevoegdScenario.Events.Union(
    [
        new EventsPerVCode(VCode, LidmaatschapWerdVerwijderd),
    ]).ToArray();
}

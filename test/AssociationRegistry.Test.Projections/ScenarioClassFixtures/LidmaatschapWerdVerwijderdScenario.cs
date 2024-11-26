namespace AssociationRegistry.Test.Projections.ScenarioClassFixtures;

using Events;
using Framework;

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

    public override EventsPerVCode[] Events => _werdToegevoegdScenario.Events.Union(
    [
        new(_werdToegevoegdScenario.VerenigingWerdGeregistreerd.VCode, LidmaatschapWerdVerwijderd),
    ]).ToArray();
}

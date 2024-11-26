namespace AssociationRegistry.Test.Projections.Scenarios;

using AutoFixture;
using Common.AutoFixture;
using Events;

public class LidmaatschapWerdGewijzigdScenario : IScenario
{
    private readonly LidmaatschapWerdToegevoegdScenario _scenario;
    public LidmaatschapWerdGewijzigd LidmaatschapWerdGewijzigd { get; }

    public LidmaatschapWerdGewijzigdScenario()
    {
        var fixture = new Fixture().CustomizeDomain();

        _scenario = new LidmaatschapWerdToegevoegdScenario();

        LidmaatschapWerdGewijzigd = new LidmaatschapWerdGewijzigd(
            Lidmaatschap: fixture.Create<Registratiedata.Lidmaatschap>() with
            {
                LidmaatschapId = _scenario.LidmaatschapWerdToegevoegd.Lidmaatschap.LidmaatschapId,
            },
            VCode: _scenario.VerenigingWerdGeregistreerd.VCode);
    }

    public string VCode => LidmaatschapWerdGewijzigd.VCode;

    public IEnumerable<EventsPerVCode> GivenEvents =>
    _scenario.GivenEvents.Append(
        new EventsPerVCode(LidmaatschapWerdGewijzigd.VCode,
                           LidmaatschapWerdGewijzigd));
}

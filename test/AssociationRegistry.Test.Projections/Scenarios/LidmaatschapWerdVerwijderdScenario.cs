namespace AssociationRegistry.Test.Projections.Scenarios;

using Events;

public class LidmaatschapWerdVerwijderdScenario : IScenario
{
    private readonly LidmaatschapWerdToegevoegdScenario _scenario;
    public LidmaatschapWerdVerwijderd LidmaatschapWerdVerwijderd { get; }

    public LidmaatschapWerdVerwijderdScenario()
    {
        _scenario = new LidmaatschapWerdToegevoegdScenario();

        LidmaatschapWerdVerwijderd = new LidmaatschapWerdVerwijderd(
            Lidmaatschap: _scenario.LidmaatschapWerdToegevoegd.Lidmaatschap,
            VCode: _scenario.VerenigingWerdGeregistreerd.VCode);
    }

    public string VCode => LidmaatschapWerdVerwijderd.VCode;

    public IEnumerable<EventsPerVCode> GivenEvents =>
        _scenario.GivenEvents.Append(new EventsPerVCode(LidmaatschapWerdVerwijderd.VCode,
                                                        LidmaatschapWerdVerwijderd));
}

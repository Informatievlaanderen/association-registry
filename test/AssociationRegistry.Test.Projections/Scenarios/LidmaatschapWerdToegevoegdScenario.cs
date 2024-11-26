namespace AssociationRegistry.Test.Projections.Scenarios;

using AutoFixture;
using Common.AutoFixture;
using Events;

public class LidmaatschapWerdToegevoegdScenario : IScenario
{
    public FeitelijkeVerenigingWerdGeregistreerd VerenigingWerdGeregistreerd { get; }
    public LidmaatschapWerdToegevoegd LidmaatschapWerdToegevoegd { get; set; }

    public LidmaatschapWerdToegevoegdScenario()
    {
        var fixture = new Fixture().CustomizeDomain();
        VerenigingWerdGeregistreerd = fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>();

        LidmaatschapWerdToegevoegd = fixture.Create<LidmaatschapWerdToegevoegd>();
    }

    public string VCode => VerenigingWerdGeregistreerd.VCode;

    public IEnumerable<EventsPerVCode> GivenEvents =>
    [
        new EventsPerVCode(VerenigingWerdGeregistreerd.VCode,
                           VerenigingWerdGeregistreerd, LidmaatschapWerdToegevoegd),
    ];
}

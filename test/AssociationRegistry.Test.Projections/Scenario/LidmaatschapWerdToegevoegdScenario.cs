namespace AssociationRegistry.Test.Projections.Scenario;

using AutoFixture;
using Events;

public class LidmaatschapWerdToegevoegdScenario : ScenarioBase
{
    public FeitelijkeVerenigingWerdGeregistreerd VerenigingWerdGeregistreerd { get; }
    public LidmaatschapWerdToegevoegd LidmaatschapWerdToegevoegd { get; set; }

    public LidmaatschapWerdToegevoegdScenario()
    {
        VerenigingWerdGeregistreerd = AutoFixture.Create<FeitelijkeVerenigingWerdGeregistreerd>();

        LidmaatschapWerdToegevoegd = AutoFixture.Create<LidmaatschapWerdToegevoegd>() with
        {
            VCode = VerenigingWerdGeregistreerd.VCode,
        };
    }

    public override string VCode => VerenigingWerdGeregistreerd.VCode;

    public override EventsPerVCode[] Events =>
    [
        new(VCode, VerenigingWerdGeregistreerd, LidmaatschapWerdToegevoegd),
    ];
}

namespace AssociationRegistry.Test.Projections.ScenarioClassFixtures;

using AutoFixture;
using Events;
using Framework;

public class LidmaatschapWerdToegevoegdScenario : ScenarioBase
{
    public FeitelijkeVerenigingWerdGeregistreerd VerenigingWerdGeregistreerd { get; }
    public LidmaatschapWerdToegevoegd LidmaatschapWerdToegevoegd { get; set; }

    public LidmaatschapWerdToegevoegdScenario()
    {
        VerenigingWerdGeregistreerd = AutoFixture.Create<FeitelijkeVerenigingWerdGeregistreerd>();
        LidmaatschapWerdToegevoegd = AutoFixture.Create<LidmaatschapWerdToegevoegd>();
    }

    public override EventsPerVCode[] Events =>
    [
        new(VerenigingWerdGeregistreerd.VCode, VerenigingWerdGeregistreerd, LidmaatschapWerdToegevoegd),
    ];
}

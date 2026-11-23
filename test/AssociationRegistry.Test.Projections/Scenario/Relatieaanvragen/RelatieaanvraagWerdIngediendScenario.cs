namespace AssociationRegistry.Test.Projections.Scenario.Relatieaanvragen;

using AutoFixture;
using Events;

/// <summary>A relatieaanvraag has its own stream: the aanvraagId.</summary>
public class RelatieaanvraagWerdIngediendScenario : ScenarioBase
{
    public RelatieaanvraagWerdIngediend RelatieaanvraagWerdIngediend { get; }

    public RelatieaanvraagWerdIngediendScenario()
    {
        RelatieaanvraagWerdIngediend = AutoFixture.Create<RelatieaanvraagWerdIngediend>() with
        {
            AanvraagId = $"RA{AutoFixture.Create<int>() % 10_000_000:0000000}",
            KoepelVCode = AutoFixture.Create<string>(),
            VerenigingVCode = AutoFixture.Create<string>(),
            RelatieType = "Lidmaatschap",
        };
    }

    public override string AggregateId => RelatieaanvraagWerdIngediend.AanvraagId;

    public override EventsPerVCode[] Events => [new(AggregateId, RelatieaanvraagWerdIngediend)];
}

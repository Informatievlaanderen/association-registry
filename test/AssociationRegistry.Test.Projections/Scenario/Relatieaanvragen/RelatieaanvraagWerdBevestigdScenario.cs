namespace AssociationRegistry.Test.Projections.Scenario.Relatieaanvragen;

using AutoFixture;
using Events;

/// <summary>A relatieaanvraag that was ingediend and then bevestigd, in its own stream.</summary>
public class RelatieaanvraagWerdBevestigdScenario : ScenarioBase
{
    public RelatieaanvraagWerdIngediendScenario Ingediend { get; } = new();
    public RelatieaanvraagWerdBevestigd RelatieaanvraagWerdBevestigd { get; }

    public RelatieaanvraagWerdBevestigdScenario()
    {
        RelatieaanvraagWerdBevestigd = AutoFixture.Create<RelatieaanvraagWerdBevestigd>() with
        {
            AanvraagId = Ingediend.AggregateId,
        };
    }

    public override string AggregateId => Ingediend.AggregateId;

    public override EventsPerVCode[] Events =>
        [new(AggregateId, Ingediend.RelatieaanvraagWerdIngediend, RelatieaanvraagWerdBevestigd)];
}

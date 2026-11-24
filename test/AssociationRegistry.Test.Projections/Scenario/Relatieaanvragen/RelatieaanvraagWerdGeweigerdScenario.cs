namespace AssociationRegistry.Test.Projections.Scenario.Relatieaanvragen;

using AutoFixture;
using Events;

/// <summary>A relatieaanvraag that was ingediend and then geweigerd, in its own stream.</summary>
public class RelatieaanvraagWerdGeweigerdScenario : ScenarioBase
{
    public RelatieaanvraagWerdIngediendScenario Ingediend { get; } = new();
    public RelatieaanvraagWerdGeweigerd RelatieaanvraagWerdGeweigerd { get; }

    public RelatieaanvraagWerdGeweigerdScenario()
    {
        RelatieaanvraagWerdGeweigerd = AutoFixture.Create<RelatieaanvraagWerdGeweigerd>() with
        {
            AanvraagId = Ingediend.AggregateId,
        };
    }

    public override string AggregateId => Ingediend.AggregateId;

    public override EventsPerVCode[] Events =>
        [new(AggregateId, Ingediend.RelatieaanvraagWerdIngediend, RelatieaanvraagWerdGeweigerd)];
}

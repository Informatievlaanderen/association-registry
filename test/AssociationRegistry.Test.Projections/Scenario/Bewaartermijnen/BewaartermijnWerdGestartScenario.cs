namespace AssociationRegistry.Test.Projections.Scenario.Bewaartermijnen;

using AutoFixture;
using Events;

public class BewaartermijnWerdGestartScenario : ScenarioBase
{
    public BewaartermijnWerdGestartV2 BewaartermijnWerdGestartV2 { get; }

    public BewaartermijnWerdGestartScenario()
    {
        BewaartermijnWerdGestartV2 = AutoFixture.Create<BewaartermijnWerdGestartV2>();
    }

    public override string AggregateId => BewaartermijnWerdGestartV2.BewaartermijnId;

    public override EventsPerVCode[] Events => [new(AggregateId, BewaartermijnWerdGestartV2)];
}

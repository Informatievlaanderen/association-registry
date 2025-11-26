namespace AssociationRegistry.Test.Projections.Scenario.Bewaartermijnen;

using AutoFixture;
using Events;

public class BewaartermijnWerdGestartScenario : ScenarioBase
{
    public BewaartermijnWerdGestart BewaartermijnWerdGestart { get; }

    public BewaartermijnWerdGestartScenario()
    {
        BewaartermijnWerdGestart = AutoFixture.Create<BewaartermijnWerdGestart>();
    }

    public override string AggregateId => BewaartermijnWerdGestart.BewaartermijnId;

    public override EventsPerVCode[] Events =>
    [
        new(AggregateId, BewaartermijnWerdGestart),
    ];
}

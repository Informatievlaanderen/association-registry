namespace AssociationRegistry.Test.Projections.Scenario.Relatieaanvragen;

using AutoFixture;
using Events;

/// <summary>A relatieaanvraag that was ingediend and then ingetrokken, in its own stream.</summary>
public class RelatieaanvraagWerdIngetrokkenScenario : ScenarioBase
{
    public RelatieaanvraagWerdIngediendScenario Ingediend { get; } = new();
    public RelatieaanvraagWerdIngetrokken RelatieaanvraagWerdIngetrokken { get; }

    public RelatieaanvraagWerdIngetrokkenScenario()
    {
        RelatieaanvraagWerdIngetrokken = AutoFixture.Create<RelatieaanvraagWerdIngetrokken>() with
        {
            AanvraagId = Ingediend.AggregateId,
        };
    }

    public override string AggregateId => Ingediend.AggregateId;

    public override EventsPerVCode[] Events =>
        [new(AggregateId, Ingediend.RelatieaanvraagWerdIngediend, RelatieaanvraagWerdIngetrokken)];
}

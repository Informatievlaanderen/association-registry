namespace AssociationRegistry.Test.Projections.Scenario.Relatieaanvragen;

using Events;

/// <summary>
///     A relatieaanvraag that was ingediend and then vervallen, in its own stream. The job ran a few days after the
///     vervaldatum.
/// </summary>
public class RelatieaanvraagWerdVervallenScenario : ScenarioBase
{
    public RelatieaanvraagWerdIngediendScenario Ingediend { get; } = new();
    public RelatieaanvraagWerdVervallen RelatieaanvraagWerdVervallen { get; }

    public RelatieaanvraagWerdVervallenScenario()
    {
        RelatieaanvraagWerdVervallen = new RelatieaanvraagWerdVervallen(
            Ingediend.AggregateId,
            Ingediend.RelatieaanvraagWerdIngediend.VervalDatum.AddDays(3)
        );
    }

    public override string AggregateId => Ingediend.AggregateId;

    public override EventsPerVCode[] Events =>
        [new(AggregateId, Ingediend.RelatieaanvraagWerdIngediend, RelatieaanvraagWerdVervallen)];
}

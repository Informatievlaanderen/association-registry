namespace AssociationRegistry.Test.Projections.Scenario.Dubbels;

using Events;
using static AssociationRegistry.Test.Common.DubbelMerge.TestVertegenwoordigers;

/// <summary>
///     Zelfde samenvoeging als <see cref="DubbeleVerenigingWerdSamengevoegdWithNewItemsScenario" />, bekeken vanuit
///     de INSZ van de overgenomen vertegenwoordiger (Samwise).
/// </summary>
#pragma warning disable CS0618
public class DubbeleVerenigingWerdSamengevoegdForInszScenario : InszScenarioBase
{
    private readonly DubbeleVerenigingWerdSamengevoegdWithNewItemsScenario _scenario = new();

    public VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd AuthentiekeVerenigingWerdGeregistreerd =>
        _scenario.AuthentiekeVerenigingWerdGeregistreerd;

    public DubbeleVerenigingWerdSamengevoegdMetAuthentiekeVereniging Samengevoegd => _scenario.Samengevoegd;

    public override string AggregateId => _scenario.AggregateId;
    public override string Insz => Samwise.Insz;
    public override EventsPerVCode[] Events => _scenario.Events;
}
#pragma warning restore CS0618

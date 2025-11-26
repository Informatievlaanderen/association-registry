namespace AssociationRegistry.Test.Projections.Scenario.Locaties;

using Events;
using AutoFixture;

public class LocatieDuplicaatWerdVerwijderdNaAdresMatchScenario : ScenarioBase
{
    public VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd { get; }
    public LocatieDuplicaatWerdVerwijderdNaAdresMatch LocatieDuplicaatWerdVerwijderdNaAdresMatch { get; }

    public LocatieDuplicaatWerdVerwijderdNaAdresMatchScenario()
    {
        VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd =
            AutoFixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>();

        LocatieDuplicaatWerdVerwijderdNaAdresMatch = AutoFixture.Create<LocatieDuplicaatWerdVerwijderdNaAdresMatch>() with
        {
            VerwijderdeLocatieId = VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.Locaties[0].LocatieId,
            BehoudenLocatieId = VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.Locaties[1].LocatieId,
        };
    }

    public override string AggregateId => VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode;

    public override EventsPerVCode[] Events =>
    [
        new(VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode,
            VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd,
            LocatieDuplicaatWerdVerwijderdNaAdresMatch
        ),
    ];
}

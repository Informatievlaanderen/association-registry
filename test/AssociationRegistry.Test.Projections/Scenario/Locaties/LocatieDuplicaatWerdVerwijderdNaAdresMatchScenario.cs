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
            VerwijderdeLocatieId = VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.Locaties.First().LocatieId,
        };
    }

    public override string VCode => VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode;

    public override EventsPerVCode[] Events =>
    [
        new(VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode,
            VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd,
            LocatieDuplicaatWerdVerwijderdNaAdresMatch
        ),
    ];
}

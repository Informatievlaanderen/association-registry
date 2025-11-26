namespace AssociationRegistry.Test.Projections.Scenario.Adressen;

using Events;
using AutoFixture;

public class AdresWerdGewijzigdInAdressenregisterScenario : ScenarioBase
{
    public VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd { get; }
    public AdresWerdGewijzigdInAdressenregister AdresWerdGewijzigdInAdressenregister { get; }

    public AdresWerdGewijzigdInAdressenregisterScenario()
    {
        VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd = AutoFixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>();
        AdresWerdGewijzigdInAdressenregister = AutoFixture.Create<AdresWerdGewijzigdInAdressenregister>() with
        {
            VCode = VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode,
            LocatieId = VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.Locaties.First().LocatieId,
        };
    }

    public override string AggregateId => VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode;

    public override EventsPerVCode[] Events =>
    [
        new(AggregateId, VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd, AdresWerdGewijzigdInAdressenregister),
    ];
}

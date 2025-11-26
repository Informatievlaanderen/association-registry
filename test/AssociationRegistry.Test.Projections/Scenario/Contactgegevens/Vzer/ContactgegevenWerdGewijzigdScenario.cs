namespace AssociationRegistry.Test.Projections.Scenario.Contactgegevens.Vzer;

using AutoFixture;
using Events;

public class ContactgegevenWerdGewijzigdScenario : ScenarioBase
{
    public VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd { get; }
    public ContactgegevenWerdGewijzigd ContactgegevenWerdGewijzigd { get; }

    public ContactgegevenWerdGewijzigdScenario()
    {
        VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd =
            AutoFixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>();

        ContactgegevenWerdGewijzigd = AutoFixture.Create<ContactgegevenWerdGewijzigd>() with
        {
            ContactgegevenId = VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.Contactgegevens.First().ContactgegevenId,
        };
    }

    public override string AggregateId => VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode;

    public override EventsPerVCode[] Events =>
    [
        new(VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode, VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd,
            ContactgegevenWerdGewijzigd),
    ];
}

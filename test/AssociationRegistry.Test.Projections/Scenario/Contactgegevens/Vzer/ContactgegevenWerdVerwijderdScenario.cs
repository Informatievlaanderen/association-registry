namespace AssociationRegistry.Test.Projections.Scenario.Contactgegevens.Vzer;

using AutoFixture;
using Events;

public class ContactgegevenWerdVerwijderdScenario : ScenarioBase
{
    public VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd { get; }
    public ContactgegevenWerdVerwijderd ContactgegevenWerdVerwijderd { get; }

    public ContactgegevenWerdVerwijderdScenario()
    {
        VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd =
            AutoFixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>();

        ContactgegevenWerdVerwijderd = AutoFixture.Create<ContactgegevenWerdVerwijderd>() with
        {
            ContactgegevenId = VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.Contactgegevens.First().ContactgegevenId,
        };
    }

    public override string AggregateId => VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode;

    public override EventsPerVCode[] Events =>
    [
        new(VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode,
            VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd,
            ContactgegevenWerdVerwijderd
        ),
    ];
}

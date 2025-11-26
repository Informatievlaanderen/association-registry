namespace AssociationRegistry.Test.Projections.Scenario.Contactgegevens.Vzer;

using AutoFixture;
using Events;

public class ContactgegevenWerdToegevoegdScenario : ScenarioBase
{
    public VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd { get; }
    public ContactgegevenWerdToegevoegd ContactgegevenWerdToegevoegd { get; }

    public ContactgegevenWerdToegevoegdScenario()
    {
        VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd =
            AutoFixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>();

        ContactgegevenWerdToegevoegd = AutoFixture.Create<ContactgegevenWerdToegevoegd>();
    }

    public override string AggregateId => VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode;

    public override EventsPerVCode[] Events =>
    [
        new(VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode,
            VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd,
            ContactgegevenWerdToegevoegd
        ),
    ];
}

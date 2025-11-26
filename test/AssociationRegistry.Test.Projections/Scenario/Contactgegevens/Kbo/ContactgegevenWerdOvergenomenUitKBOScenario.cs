namespace AssociationRegistry.Test.Projections.Scenario.Contactgegevens.Kbo;

using Events;
using AutoFixture;

public class ContactgegevenWerdOvergenomenUitKBOScenario : ScenarioBase
{
    public VerenigingMetRechtspersoonlijkheidWerdGeregistreerd VerenigingMetRechtspersoonlijkheidWerdGeregistreerd { get; }
    public ContactgegevenWerdOvergenomenUitKBO ContactgegevenWerdOvergenomenUitKbo { get; }

    public ContactgegevenWerdOvergenomenUitKBOScenario()
    {
        VerenigingMetRechtspersoonlijkheidWerdGeregistreerd = AutoFixture.Create<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>();
        ContactgegevenWerdOvergenomenUitKbo = AutoFixture.Create<ContactgegevenWerdOvergenomenUitKBO>();
    }

    public override string AggregateId => VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.VCode;

    public override EventsPerVCode[] Events =>
    [
        new(AggregateId, VerenigingMetRechtspersoonlijkheidWerdGeregistreerd, ContactgegevenWerdOvergenomenUitKbo),
    ];
}

namespace AssociationRegistry.Test.Projections.Scenario.Contactgegevens;

using AutoFixture;
using Events;

public class ContactgegevenWerdOvergenomenUitKBOScenario : ScenarioBase
{
    public VerenigingMetRechtspersoonlijkheidWerdGeregistreerd VerenigingMetRechtspersoonlijkheidWerdGeregistreerd { get; }
    public ContactgegevenWerdOvergenomenUitKBO ContactgegevenWerdOvergenomenUitKbo { get; }

    public ContactgegevenWerdOvergenomenUitKBOScenario()
    {
        VerenigingMetRechtspersoonlijkheidWerdGeregistreerd = AutoFixture.Create<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>();
        ContactgegevenWerdOvergenomenUitKbo = AutoFixture.Create<ContactgegevenWerdOvergenomenUitKBO>();
    }

    public override string VCode => VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.VCode;

    public override EventsPerVCode[] Events =>
    [
        new(VCode, VerenigingMetRechtspersoonlijkheidWerdGeregistreerd, ContactgegevenWerdOvergenomenUitKbo),
    ];
}

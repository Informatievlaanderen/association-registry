namespace AssociationRegistry.Test.Projections.Scenario.Contactgegevens.Kbo;

using AssociationRegistry.Events;
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

    public override string VCode => VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.VCode;

    public override EventsPerVCode[] Events =>
    [
        new(VCode, VerenigingMetRechtspersoonlijkheidWerdGeregistreerd, ContactgegevenWerdOvergenomenUitKbo),
    ];
}

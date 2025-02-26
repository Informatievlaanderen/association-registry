namespace AssociationRegistry.Test.Projections.Scenario.Contactgegevens;

using AutoFixture;
using Events;

public class ContactgegevenWerdGewijzigdInKboScenario : ScenarioBase
{
    public readonly ContactgegevenWerdOvergenomenUitKBOScenario _contactgegevenWerdOvergenomenUitKboScenario = new();
    public ContactgegevenWerdGewijzigdInKbo ContactgegevenWerdGewijzigdInKbo { get; }

    public ContactgegevenWerdGewijzigdInKboScenario()
    {
        ContactgegevenWerdGewijzigdInKbo = AutoFixture.Create<ContactgegevenWerdGewijzigdInKbo>() with
        {
            ContactgegevenId = _contactgegevenWerdOvergenomenUitKboScenario.ContactgegevenWerdOvergenomenUitKbo.ContactgegevenId,
        };
    }

    public override string VCode => _contactgegevenWerdOvergenomenUitKboScenario.VCode;

    public override EventsPerVCode[] Events => _contactgegevenWerdOvergenomenUitKboScenario.Events.Union(
    [
        new EventsPerVCode(VCode, ContactgegevenWerdGewijzigdInKbo),
    ]).ToArray();
}

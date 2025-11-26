namespace AssociationRegistry.Test.Projections.Scenario.Contactgegevens.Kbo;

using Events;
using AutoFixture;

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

    public override string AggregateId => _contactgegevenWerdOvergenomenUitKboScenario.AggregateId;

    public override EventsPerVCode[] Events => _contactgegevenWerdOvergenomenUitKboScenario.Events.Union(
    [
        new EventsPerVCode(AggregateId, ContactgegevenWerdGewijzigdInKbo),
    ]).ToArray();
}

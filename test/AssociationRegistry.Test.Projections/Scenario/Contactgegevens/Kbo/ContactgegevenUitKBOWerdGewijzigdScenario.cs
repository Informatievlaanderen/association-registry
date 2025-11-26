namespace AssociationRegistry.Test.Projections.Scenario.Contactgegevens.Kbo;

using Events;
using AutoFixture;

public class ContactgegevenUitKBOWerdGewijzigdScenario : ScenarioBase
{
    public readonly ContactgegevenWerdOvergenomenUitKBOScenario _contactgegevenWerdOvergenomenUitKboScenario = new();
    public ContactgegevenUitKBOWerdGewijzigd ContactgegevenUitKBOWerdGewijzigd { get; }

    public ContactgegevenUitKBOWerdGewijzigdScenario()
    {
        ContactgegevenUitKBOWerdGewijzigd = AutoFixture.Create<ContactgegevenUitKBOWerdGewijzigd>() with
        {
            ContactgegevenId = _contactgegevenWerdOvergenomenUitKboScenario.ContactgegevenWerdOvergenomenUitKbo.ContactgegevenId,
        };
    }

    public override string AggregateId => _contactgegevenWerdOvergenomenUitKboScenario.AggregateId;

    public override EventsPerVCode[] Events => _contactgegevenWerdOvergenomenUitKboScenario.Events.Union(
    [
        new EventsPerVCode(AggregateId, ContactgegevenUitKBOWerdGewijzigd),
    ]).ToArray();
}

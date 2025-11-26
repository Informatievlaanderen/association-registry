namespace AssociationRegistry.Test.Projections.Scenario.Contactgegevens.Kbo;

using Events;
using AutoFixture;

public class ContactgegevenWerdInBeheerGenomenDoorKboScenario : ScenarioBase
{
    public readonly ContactgegevenWerdOvergenomenUitKBOScenario _contactgegevenWerdOvergenomenUitKboScenario = new();
    public ContactgegevenWerdInBeheerGenomenDoorKbo ContactgegevenWerdInBeheerGenomenDoorKbo { get; }

    public ContactgegevenWerdInBeheerGenomenDoorKboScenario()
    {
        ContactgegevenWerdInBeheerGenomenDoorKbo = AutoFixture.Create<ContactgegevenWerdInBeheerGenomenDoorKbo>() with
        {
            ContactgegevenId = _contactgegevenWerdOvergenomenUitKboScenario.ContactgegevenWerdOvergenomenUitKbo.ContactgegevenId,
        };
    }

    public override string AggregateId => _contactgegevenWerdOvergenomenUitKboScenario.AggregateId;

    public override EventsPerVCode[] Events => _contactgegevenWerdOvergenomenUitKboScenario.Events.Union(
    [
        new EventsPerVCode(AggregateId, ContactgegevenWerdInBeheerGenomenDoorKbo),
    ]).ToArray();
}

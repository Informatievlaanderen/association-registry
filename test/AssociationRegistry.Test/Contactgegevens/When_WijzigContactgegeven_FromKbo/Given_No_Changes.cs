namespace AssociationRegistry.Test.Contactgegevens.When_WijzigContactgegeven_FromKbo;

using AssociationRegistry.Events;
using AssociationRegistry.Test.Common.AutoFixture;
using AutoFixture;
using DecentraalBeheer.Vereniging;
using FluentAssertions;
using Xunit;

public class Given_No_Changes
{
    [Fact]
    public void Then_It_Does_Not_Emit_Events()
    {
        var fixture = new Fixture().CustomizeDomain();

        var vereniging = new VerenigingMetRechtspersoonlijkheid();
        var contactgegevenWerdOvergenomenUitKbo = fixture.Create<ContactgegevenWerdOvergenomenUitKBO>();

        var contactgegevenUitKboWerdGewijzigd = fixture.Create<ContactgegevenUitKBOWerdGewijzigd>() with
        {
            ContactgegevenId = contactgegevenWerdOvergenomenUitKbo.ContactgegevenId,
        };

        vereniging.Hydrate(new VerenigingState()
                          .Apply(fixture.Create<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>())
                          .Apply(contactgegevenWerdOvergenomenUitKbo)
                          .Apply(contactgegevenUitKboWerdGewijzigd)
        );

        vereniging.WijzigContactgegeven(contactgegevenUitKboWerdGewijzigd.ContactgegevenId, contactgegevenUitKboWerdGewijzigd.Beschrijving,
                                        contactgegevenUitKboWerdGewijzigd.IsPrimair);

        vereniging.UncommittedEvents.Should().BeEmpty();
    }
}

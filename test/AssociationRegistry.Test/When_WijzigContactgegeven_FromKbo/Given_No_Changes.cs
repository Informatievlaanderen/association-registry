namespace AssociationRegistry.Test.When_WijzigContactgegeven_FromKbo;

using AutoFixture;
using Common.AutoFixture;
using Events;
using FluentAssertions;
using Framework.Customizations;
using Vereniging;
using Xunit;
using Xunit.Categories;

[UnitTest]
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

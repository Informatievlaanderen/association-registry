namespace AssociationRegistry.Test.When_WijzigContactgegeven_FromKbo;

using Events;
using Framework.Customizations;
using Vereniging;
using AutoFixture;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_A_Contactgegeven_From_Kbo
{
    [Fact]
    public void Then_It_Emits_An_Event()
    {
        var fixture = new Fixture().CustomizeDomain();

        var vereniging = new VerenigingMetRechtspersoonlijkheid();
        var contactgegevenWerdOvergenomenUitKbo = fixture.Create<ContactgegevenWerdOvergenomenUitKBO>();

        vereniging.Hydrate(new VerenigingState()
                          .Apply(fixture.Create<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>())
                          .Apply(contactgegevenWerdOvergenomenUitKbo)
        );

        var beschrijving = fixture.Create<string>();
        var isPrimair = fixture.Create<bool>();

        vereniging.WijzigContactgegeven(contactgegevenWerdOvergenomenUitKbo.ContactgegevenId, beschrijving,
                                        isPrimair);

        vereniging.UncommittedEvents.Should().BeEquivalentTo(new[]
        {
            new ContactgegevenVolgensKBOWerdGewijzigd(contactgegevenWerdOvergenomenUitKbo.ContactgegevenId, beschrijving,
                                                      isPrimair),
        });
    }
}

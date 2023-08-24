namespace AssociationRegistry.Test.When_WijzigContactgegeven;

using AssociationRegistry.Events;
using AssociationRegistry.Test.Framework.Customizations;
using AssociationRegistry.Vereniging;
using AutoFixture;
using FluentAssertions;
using Vereniging.Exceptions;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_A_Contactgegeven_From_Kbo
{
    [Fact]
    public void Then_It_Throws()
    {
        var fixture = new Fixture().CustomizeDomain();

        var vereniging = new VerenigingOfAnyKind();
        var contactgegevenWerdOvergenomenUitKbo = fixture.Create<ContactgegevenWerdOvergenomenUitKBO>();
        vereniging.Hydrate(new VerenigingState()
            .Apply(fixture.Create<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>())
            .Apply(contactgegevenWerdOvergenomenUitKbo)
        );

        var wijzigContactgegen = ()=>vereniging.WijzigContactgegeven(contactgegevenWerdOvergenomenUitKbo.ContactgegevenId, fixture.Create<string>(), fixture.Create<string>(), fixture.Create<bool>());

        wijzigContactgegen.Should().Throw<ContactgegevenFromKboCannotBeUpdated>();

    }
}

namespace AssociationRegistry.Test.Contactgegevens.When_VerwijderContactgegeven;

using AssociationRegistry.Events;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Vereniging;
using AssociationRegistry.Vereniging.Exceptions;
using AutoFixture;
using FluentAssertions;
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

        var contactgegevenWerdOvergenomenUitKbo = fixture.Create<ContactgegevenWerdOvergenomenUitKBO>() with { ContactgegevenId = 1 };

        vereniging.Hydrate(new VerenigingState()
                          .Apply(fixture.Create<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>())
                          .Apply(contactgegevenWerdOvergenomenUitKbo));

        var wijzigLocatie = () => vereniging.VerwijderContactgegeven(contactgegevenWerdOvergenomenUitKbo.ContactgegevenId);

        wijzigLocatie.Should().Throw<ContactgegevenUitKboKanNietVerwijderdWorden>();
    }
}

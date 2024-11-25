namespace AssociationRegistry.Test.When_VerwijderContactgegeven;

using AutoFixture;
using Common.AutoFixture;
using Events;
using FluentAssertions;
using Framework.Customizations;
using Vereniging;
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

        var contactgegevenWerdOvergenomenUitKbo = fixture.Create<ContactgegevenWerdOvergenomenUitKBO>() with { ContactgegevenId = 1 };

        vereniging.Hydrate(new VerenigingState()
                          .Apply(fixture.Create<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>())
                          .Apply(contactgegevenWerdOvergenomenUitKbo));

        var wijzigLocatie = () => vereniging.VerwijderContactgegeven(contactgegevenWerdOvergenomenUitKbo.ContactgegevenId);

        wijzigLocatie.Should().Throw<ContactgegevenUitKboKanNietVerwijderdWorden>();
    }
}

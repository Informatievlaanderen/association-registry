namespace AssociationRegistry.Test.When_VerwijderContactgegeven;

using AutoFixture;
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
        var contactgegeven = fixture.Create<Contactgegeven>() with
        {
            ContactgegevenId = 1,
        };

        var vereniging = new VerenigingOfAnyKind();
        vereniging.Hydrate(new VerenigingState()
            .Apply(fixture.Create<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>())
            .Apply(ContactgegevenWerdOvergenomenUitKBO.With(contactgegeven, ContactgegevenTypeVolgensKbo.All.First(c=>c.ContactgegevenType==contactgegeven.Type))));

        var wijzigLocatie = () => vereniging.VerwijderContactgegeven(contactgegeven.ContactgegevenId);

        wijzigLocatie.Should().Throw<ContactgegevenUitKboKanNietVerwijderdWorden>();
    }
}

namespace AssociationRegistry.Test.When_WijzigContactgegeven_FromKbo;

using AutoFixture;
using Events;
using FluentAssertions;
using Framework.Customizations;
using Vereniging;
using Vereniging.Exceptions;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_A_Second_Primair
{
    [Fact]
    public void Then_It_Throws()
    {
        var fixture = new Fixture().CustomizeDomain();

        var vereniging = new VerenigingMetRechtspersoonlijkheid();

        var contactgegevenWerdOvergenomenUitKbo = fixture.Create<ContactgegevenWerdOvergenomenUitKBO>() with
        {
            ContactgegevenId = 2,
        };

        vereniging.Hydrate(new VerenigingState()
                          .Apply(fixture.Create<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>())
                          .Apply(ContactgegevenWerdToegevoegd.With(
                                     fixture.CreateContactgegevenVolgensType(contactgegevenWerdOvergenomenUitKbo.Type) with
                                     {
                                         ContactgegevenId = 1,
                                         IsPrimair = true,
                                     }))
                          .Apply(contactgegevenWerdOvergenomenUitKbo)
        );

        var wijzig = () => vereniging.WijzigContactgegeven(contactgegevenWerdOvergenomenUitKbo.ContactgegevenId, fixture.Create<string>(),
                                                           true);

        wijzig.Should().Throw<MultiplePrimairContactgegevens>();
    }
}

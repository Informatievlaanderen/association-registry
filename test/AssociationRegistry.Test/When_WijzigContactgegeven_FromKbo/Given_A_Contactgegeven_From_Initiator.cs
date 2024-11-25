namespace AssociationRegistry.Test.When_WijzigContactgegeven_FromKbo;

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
public class Given_A_Contactgegeven_From_Initiator
{
    [Fact]
    public void Then_It_Throws()
    {
        var fixture = new Fixture().CustomizeDomain();

        var vereniging = new VerenigingMetRechtspersoonlijkheid();
        var contactgegevenWerdToegevoegd = fixture.Create<ContactgegevenWerdToegevoegd>();

        vereniging.Hydrate(new VerenigingState()
                          .Apply(fixture.Create<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>())
                          .Apply(contactgegevenWerdToegevoegd)
        );

        var wijzig = () => vereniging.WijzigContactgegeven(contactgegevenWerdToegevoegd.ContactgegevenId, fixture.Create<string>(),
                                                           fixture.Create<bool>());

        wijzig.Should().Throw<ActieIsNietToegestaanVoorContactgegevenBron>();
    }
}

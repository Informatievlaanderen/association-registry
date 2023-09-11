namespace AssociationRegistry.Test.When_WijzigDoelgroep.MetRechtspersoonlijkheid;

using Events;
using Framework.Customizations;
using Vereniging;
using AutoFixture;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_The_Same_Doelgroep
{
    [Fact]
    public void Then_It_Does_Not_Emit()
    {
        var fixture = new Fixture().CustomizeDomain();

        var vereniging = new VerenigingMetRechtspersoonlijkheid();
        var doelgroep = fixture.Create<Doelgroep>();

        vereniging.Hydrate(
            new VerenigingState()
               .Apply(fixture.Create<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>())
               .Apply(DoelgroepWerdGewijzigd.With(doelgroep))
        );

        vereniging.WijzigDoelgroep(doelgroep);

        vereniging.UncommittedEvents.Should()
                  .BeEmpty();
    }
}

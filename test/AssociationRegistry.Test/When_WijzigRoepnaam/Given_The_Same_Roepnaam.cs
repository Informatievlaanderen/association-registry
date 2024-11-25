namespace AssociationRegistry.Test.When_WijzigRoepnaam;

using AutoFixture;
using Common.AutoFixture;
using Events;
using FluentAssertions;
using Framework.Customizations;
using Vereniging;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_The_Same_Roepnaam
{
    [Fact]
    public void Then_It_Does_Not_Emit()
    {
        var fixture = new Fixture().CustomizeDomain();

        var vereniging = new VerenigingMetRechtspersoonlijkheid();
        var roepnaam = fixture.Create<string>();

        vereniging.Hydrate(
            new VerenigingState()
               .Apply(fixture.Create<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>())
               .Apply(new RoepnaamWerdGewijzigd(roepnaam))
        );

        vereniging.WijzigRoepnaam(roepnaam);

        vereniging.UncommittedEvents.Should()
                  .BeEmpty();
    }
}

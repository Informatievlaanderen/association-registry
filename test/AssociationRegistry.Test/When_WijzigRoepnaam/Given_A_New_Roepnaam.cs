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
public class Given_A_New_Roepnaam
{
    [Fact]
    public void Then_It_Emits()
    {
        var fixture = new Fixture().CustomizeDomain();

        var vereniging = new VerenigingMetRechtspersoonlijkheid();

        vereniging.Hydrate(
            new VerenigingState()
               .Apply(fixture.Create<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>()));

        var roepnaam = fixture.Create<string>();
        vereniging.WijzigRoepnaam(roepnaam);

        vereniging.UncommittedEvents.Should()
                  .BeEquivalentTo(new[]
                   {
                       new RoepnaamWerdGewijzigd(roepnaam),
                   });
    }
}

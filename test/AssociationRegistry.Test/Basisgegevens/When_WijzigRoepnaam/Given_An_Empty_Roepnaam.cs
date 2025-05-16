namespace AssociationRegistry.Test.Basisgegevens.When_WijzigRoepnaam;

using AssociationRegistry.Events;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Vereniging;
using AutoFixture;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_An_Empty_Roepnaam
{
    [Fact]
    public void Then_It_Emits()
    {
        var fixture = new Fixture().CustomizeDomain();

        var vereniging = new VerenigingMetRechtspersoonlijkheid();

        vereniging.Hydrate(
            new VerenigingState()
               .Apply(fixture.Create<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>()));

        var roepnaam = string.Empty;
        vereniging.WijzigRoepnaam(roepnaam);

        vereniging.UncommittedEvents.Should()
                  .BeEquivalentTo(new[]
                   {
                       new RoepnaamWerdGewijzigd(roepnaam),
                   });
    }
}

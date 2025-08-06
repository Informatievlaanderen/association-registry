namespace AssociationRegistry.Test.Basisgegevens.When_WijzigRoepnaam;

using AssociationRegistry.Events;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Vereniging;
using AutoFixture;
using DecentraalBeheer.Vereniging;
using FluentAssertions;
using Xunit;

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

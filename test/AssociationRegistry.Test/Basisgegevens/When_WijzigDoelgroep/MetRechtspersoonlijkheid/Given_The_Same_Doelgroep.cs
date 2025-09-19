namespace AssociationRegistry.Test.Basisgegevens.When_WijzigDoelgroep.MetRechtspersoonlijkheid;

using AssociationRegistry.Events;
using AssociationRegistry.Test.Common.AutoFixture;
using AutoFixture;
using DecentraalBeheer.Vereniging;
using Events.Factories;
using FluentAssertions;
using Xunit;

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
               .Apply(EventFactory.DoelgroepWerdGewijzigd(doelgroep))
        );

        vereniging.WijzigDoelgroep(doelgroep);

        vereniging.UncommittedEvents.Should()
                  .BeEmpty();
    }
}

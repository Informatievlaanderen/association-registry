namespace AssociationRegistry.Test.Basisgegevens.When_WijzigDoelgroep.MetRechtspersoonlijkheid;

using AssociationRegistry.Events;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Vereniging;
using AutoFixture;
using DecentraalBeheer.Vereniging;
using Events.Factories;
using FluentAssertions;
using Xunit;

public class Given_A_New_Doelgroep
{
    [Fact]
    public void Then_It_Emits()
    {
        var fixture = new Fixture().CustomizeDomain();

        var vereniging = new VerenigingMetRechtspersoonlijkheid();

        vereniging.Hydrate(
            new VerenigingState()
               .Apply(fixture.Create<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>()));

        var doelgroep = fixture.Create<Doelgroep>();
        vereniging.WijzigDoelgroep(doelgroep);

        vereniging.UncommittedEvents.Should()
                  .BeEquivalentTo(new[]
                   {
                       EventFactory.DoelgroepWerdGewijzigd(doelgroep),
                   });
    }
}

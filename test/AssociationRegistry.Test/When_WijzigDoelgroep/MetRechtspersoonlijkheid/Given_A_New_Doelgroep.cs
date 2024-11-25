namespace AssociationRegistry.Test.When_WijzigDoelgroep.MetRechtspersoonlijkheid;

using AutoFixture;
using Common.AutoFixture;
using Events;
using FluentAssertions;
using Framework.Customizations;
using Vereniging;
using Xunit;
using Xunit.Categories;

[UnitTest]
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
                       DoelgroepWerdGewijzigd.With(doelgroep),
                   });
    }
}

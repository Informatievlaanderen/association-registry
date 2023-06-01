namespace AssociationRegistry.Test.When_Hydrating_A_Vereniging;

using FluentAssertions;
using Vereniging;
using Vereniging.Exceptions;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_An_Afdeling_As_Type
{
    [Fact]
    public void Then_It_Does_Not_Throw_For_FeitelijkeVereniging()
    {
        var hydrate = () =>
            new Vereniging().Hydrate(
                new VerenigingState
                {
                    Verenigingstype = Verenigingstype.Afdeling,
                });

        hydrate.Should().NotThrow();
    }

    [Fact]
    public void Then_It_Throws_For_KboVereniging()
    {
        var hydrate = () =>
            new VerenigingMetRechtspersoonlijkheid().Hydrate(
                new VerenigingState
                {
                    Verenigingstype = Verenigingstype.Afdeling,
                });

        hydrate.Should().Throw<UnsupportedOperationForVerenigingstype>();
    }
}

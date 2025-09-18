namespace AssociationRegistry.Test.When_Hydrating_A_Vereniging;

using DecentraalBeheer.Vereniging;
using DecentraalBeheer.Vereniging.Exceptions;
using FluentAssertions;
using Xunit;

public class Given_An_VerenigingMetRechtspersoonlijkheid_As_Type
{
    private readonly VerenigingState _verenigingState;

    public Given_An_VerenigingMetRechtspersoonlijkheid_As_Type()
    {
        _verenigingState = new VerenigingState
        {
            Verenigingstype = Verenigingstype.VZW,
        };
    }

    [Fact]
    public void Then_It_Throws_For_FeitelijkeVereniging()
    {
        var hydrate = () =>
            new Vereniging().Hydrate(_verenigingState);

        hydrate.Should().Throw<ActieIsNietToegestaanVoorVerenigingstype>();
    }

    [Fact]
    public void Then_It_Does_Not_Throw_For_VerenigingMetRechtspersoonlijkheid()
    {
        var hydrate = () =>
            new VerenigingMetRechtspersoonlijkheid().Hydrate(_verenigingState);

        hydrate.Should().NotThrow();
    }
}

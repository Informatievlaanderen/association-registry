namespace AssociationRegistry.Test.Werkingsgebied;

using FluentAssertions;
using Vereniging;
using Xunit;

public class WerkingsgebiedenTests
{
    [Fact]
    public void Werkingsgebieden_NietBepaald_Is_Empty()
    {
        Werkingsgebieden.NietBepaald.Should().BeEmpty();
    }

    [Fact]
    public void Werkingsgebieden_NietVanToepassing_Is_Not_Empty()
    {
        Werkingsgebieden.NietVanToepassing.Should().NotBeEmpty();
        Werkingsgebieden.NietVanToepassing.Should().Contain(Werkingsgebied.Create("NVT"));
    }
}

namespace AssociationRegistry.Test.When_WijzigLocatie;

using AutoFixture;
using FluentAssertions;
using Framework.Customizations;
using Vereniging;
using Vereniging.Exceptions;
using Xunit;

public class Given_A_Naam_Longer_Than_MaxLength
{
    [Fact]
    public void It_Throws_An_LocatieLengteIsOngeldig()
    {
        var fixture = new Fixture().CustomizeDomain();

        var loc = fixture.Create<Locatie>();

        var wijzigFunc = () => loc.Wijzig(new string(Enumerable.Repeat('a', 43).ToArray()));

        wijzigFunc.Should().Throw<LocatieLengteIsOngeldig>();
    }
}

namespace AssociationRegistry.Test.When_Creating_A_Locatie;

using AutoFixture;
using FluentAssertions;
using Framework.Customizations;
using Vereniging;
using Vereniging.Exceptions;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_A_Locatienaam_Longer_Than_42
{
    [Fact]
    public void Then_It_Throws_An_LocatieLengteIsOngeldig()
    {
        var fixture = new Fixture().CustomizeDomain();

        var createFunc = () => Locatie.Create(
                Locatienaam.Create(new string(Enumerable.Repeat('a', 43).ToArray())),
            fixture.Create<bool>(),
            fixture.Create<Locatietype>(),
            adresId: null,
            adres: null);

        createFunc.Should().Throw<LocatieLengteIsOngeldig>();
    }
}

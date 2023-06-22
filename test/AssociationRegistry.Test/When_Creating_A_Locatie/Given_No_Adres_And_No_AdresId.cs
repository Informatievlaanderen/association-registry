namespace AssociationRegistry.Test.When_Creating_A_Locatie;

using AutoFixture;
using FluentAssertions;
using Framework;
using Vereniging;
using Vereniging.Exceptions;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_No_Adres_And_No_AdresId
{
    [Fact]
    public void Then_It_Throws_An_MissingAdresException()
    {
        var fixture = new Fixture().CustomizeAll();
        var ctor = () => Locatie.Create(
            fixture.Create<string>(),
            fixture.Create<bool>(),
            fixture.Create<Locatietype>(),
            adresId: null,
            adres: null);
        ctor.Should().Throw<MissingAdres>();
    }
}

namespace AssociationRegistry.Test.When_Creating_An_Adres;

using AutoFixture;
using FluentAssertions;
using Vereniging;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_No_Busnummer
{
    [Fact]
    public void Then_Busnummer_Is_Empty()
    {
        var fixture = new Fixture();

        var adres = Adres.Create(fixture.Create<string>(), fixture.Create<string>(), busnummer: null, fixture.Create<string>(),
                                 fixture.Create<string>(), fixture.Create<string>());

        ((string)adres.Busnummer).Should().BeEmpty();
    }
}

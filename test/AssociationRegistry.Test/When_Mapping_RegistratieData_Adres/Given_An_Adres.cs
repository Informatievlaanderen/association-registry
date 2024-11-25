namespace AssociationRegistry.Test.When_Mapping_RegistratieData_Adres;

using AutoFixture;
using Common.AutoFixture;
using Events;
using FluentAssertions;
using Framework.Customizations;
using Vereniging;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_An_Adres
{
    [Fact]
    public void Then_It_Returns_An_Adres()
    {
        var fixture = new Fixture().CustomizeDomain();

        var adres = fixture.Create<Adres>();

        Registratiedata.Adres.With(adres)
                       .Should().Be(new Registratiedata.Adres(
                                        adres.Straatnaam,
                                        adres.Huisnummer,
                                        adres.Busnummer,
                                        adres.Postcode,
                                        adres.Gemeente.Naam,
                                        adres.Land));
    }
}

namespace AssociationRegistry.Test.Locaties.Adressen.When_Mapping_RegistratieData_Adres;

using AssociationRegistry.Events;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Vereniging;
using AutoFixture;
using EventFactories;
using FluentAssertions;
using Xunit;

public class Given_An_Adres
{
    [Fact]
    public void Then_It_Returns_An_Adres()
    {
        var fixture = new Fixture().CustomizeDomain();

        var adres = fixture.Create<Adres>();

        EventFactory.Adres(adres)
                       .Should().Be(new Registratiedata.Adres(
                                        adres.Straatnaam,
                                        adres.Huisnummer,
                                        adres.Busnummer,
                                        adres.Postcode,
                                        adres.Gemeente.Naam,
                                        adres.Land));
    }
}

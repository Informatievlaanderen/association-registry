﻿namespace AssociationRegistry.Test.When_Decorating_PostalInformation;

using Events;
using FluentAssertions;
using Grar.Models;
using Xunit;

public class With_Several_PostalNames
{
    [Fact]
    public void And_PostName_Exists_Then_Takes_The_PostalName()
    {
        var sut = new AdresMatchUitAdressenregister
        {
            Adres = new Registratiedata.Adres(Straatnaam: "Prieelstraat", Huisnummer: "12", Busnummer: "bus 101", Postcode: "1740",
                                              Gemeente: "NothingHam", Land: "België"),
        };

        var result = sut.DecorateWithPostalInformation(origineleGemeentenaam: "Hekelgem",
                                                       new PostalInformationResponse(
                                                           Postcode: "1741", Gemeentenaam: "Affligem",
                                                           new[] { "AFFLIGEM", "Essene", "Hekelgem", "Teralfene" }));

        result.Adres.Gemeente.Should().Be("Hekelgem (Affligem)");
    }

    [Fact]
    public void And_PostName_Does_Not_Exists_Then_Takes_The_MunicipalityName()
    {
        var sut = new AdresMatchUitAdressenregister
        {
            Adres = new Registratiedata.Adres(Straatnaam: "Prieelstraat", Huisnummer: "12", Busnummer: "bus 101", Postcode: "1740",
                                              Gemeente: "NothingHam", Land: "België"),
        };

        var result = sut.DecorateWithPostalInformation(origineleGemeentenaam: "Nothingham",
                                                       new PostalInformationResponse(
                                                           Postcode: "1741", Gemeentenaam: "Affligem",
                                                           new[] { "AFFLIGEM", "Essene", "Hekelgem", "Teralfene" }));

        result.Adres.Gemeente.Should().Be("Affligem");
    }
}

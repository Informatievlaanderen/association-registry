﻿namespace AssociationRegistry.Test.Locaties.Adressen.When_Formatting_An_Address;

using AssociationRegistry.Events;
using AssociationRegistry.Formats;
using FluentAssertions;
using Xunit;

public class With_A_Adres_Without_Busnummer
{
    [Fact]
    public void Then_It_Formats()
    {
        var straatnaam = "kerkstraat";
        var huisnummer = "1";
        var postcode = "007";
        var gemeente = "zonnedorp";
        var land = "fabeltjesland";

        var adres = new Registratiedata.Adres(
            straatnaam,
            huisnummer,
            string.Empty,
            postcode,
            gemeente,
            land);

        var formatted = adres.ToAdresString();
        formatted.Should().Be("kerkstraat 1, 007 zonnedorp, fabeltjesland");
    }
}

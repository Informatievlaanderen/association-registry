namespace AssociationRegistry.Test.ValueObjects.When_Creating_An_AdresId;

using AssociationRegistry.Vereniging;
using DecentraalBeheer.Vereniging.Adressen;
using DecentraalBeheer.Vereniging.Exceptions;
using FluentAssertions;
using Xunit;

public class Given_Non_DataVlaanderen_Uri_For_AR_Bron
{
    [Fact]
    public void Then_It_Throws_InvalidBronwaardeForAR()
    {
        var ctor = () => AdresId.Create(
            Adresbron.AR,
            bronwaarde: "waarde");

        ctor.Should().Throw<BronwaardeVoorAdresIsOngeldig>();
    }
}

namespace AssociationRegistry.Test.ValueObjects.When_Creating_A_Locatie;

using AssociationRegistry.Test.Common.AutoFixture;
using AutoFixture;
using DecentraalBeheer.Vereniging;
using DecentraalBeheer.Vereniging.Exceptions;
using FluentAssertions;
using Xunit;

public class Given_No_Adres_And_No_AdresId
{
    [Fact]
    public void Then_It_Throws_An_MissingAdresException()
    {
        var fixture = new Fixture().CustomizeDomain();

        var ctor = () => Locatie.Create(
            fixture.Create<Locatienaam>(),
            fixture.Create<bool>(),
            fixture.Create<Locatietype>(),
            adresId: null,
            adres: null);

        ctor.Should().Throw<AdresOfAdresIdMoetAanwezigZijn>();
    }
}

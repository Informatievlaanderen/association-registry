namespace AssociationRegistry.Test.ValueObjects.When_Creating_An_Adres;

using AssociationRegistry.Vereniging;
using AutoFixture;
using DecentraalBeheer.Vereniging.Adressen;
using FluentAssertions;
using Xunit;

public class Given_No_Busnummer
{
    [Fact]
    public void Then_Busnummer_Is_Empty()
    {
        var fixture = new Fixture();

        var adres = Adres.Create(fixture.Create<string>(), fixture.Create<string>(), busnummer: null, fixture.Create<string>(),
                                 fixture.Create<string>(), fixture.Create<string>());

        adres.Busnummer.Should().BeEmpty();
    }
}

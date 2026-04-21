namespace AssociationRegistry.Test.Erkenningen;

using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen;
using AssociationRegistry.DecentraalBeheer.Vereniging.Exceptions;
using AutoFixture;
using Common.AutoFixture;
using FluentAssertions;
using Resources;
using Xunit;

public class ErkenningsPeriodeTests
{
    private DateOnly _baseDataOnly;

    public ErkenningsPeriodeTests()
    {
        var fixture = new Fixture().CustomizeDomain();
        _baseDataOnly = fixture.Create<DateOnly>();
    }

    [Fact]
    public void Given_Empty_Startdatum_And_Empty_Einddatum_Then_Create_An_ErkenningsPeriode()
    {
        DateOnly? startdatum = null;
        DateOnly? einddatum = null;

        var periode = ErkenningsPeriode.Create(startdatum, einddatum);

        periode.Startdatum.Should().Be(startdatum);
        periode.Einddatum.Should().Be(einddatum);
    }

    [Fact]
    public void Given_Startdatum_And_Empty_Einddatum_Then_Create_Succeeds()
    {
        DateOnly? startdatum = _baseDataOnly;
        DateOnly? einddatum = null;

        var periode = ErkenningsPeriode.Create(startdatum, einddatum);

        periode.Startdatum.Should().Be(startdatum);
        periode.Einddatum.Should().Be(einddatum);
    }

    [Fact]
    public void Given_Empty_Startdatum_And_Einddatum_Then_Create_Succeeds()
    {
        DateOnly? startdatum = null;
        DateOnly? einddatum = _baseDataOnly;

        var periode = ErkenningsPeriode.Create(startdatum, einddatum);

        periode.Startdatum.Should().Be(startdatum);
        periode.Einddatum.Should().Be(einddatum);
    }

    [Fact]
    public void Given_Startdatum_Equals_Einddatum_Then_Create_Succeeds()
    {
        DateOnly? startdatum = _baseDataOnly;
        DateOnly? einddatum = _baseDataOnly;

        var periode = ErkenningsPeriode.Create(startdatum, einddatum);

        periode.Startdatum.Should().Be(startdatum);
        periode.Einddatum.Should().Be(einddatum);
    }

    [Fact]
    public void Given_Startdatum_Before_Einddatum_Then_Create_Succeeds()
    {
        DateOnly? startdatum = _baseDataOnly;
        DateOnly? einddatum = _baseDataOnly.AddDays(5);

        var periode = ErkenningsPeriode.Create(startdatum, einddatum);

        periode.Startdatum.Should().Be(startdatum);
        periode.Einddatum.Should().Be(einddatum);
    }

    [Fact]
    public void Given_Startdatum_After_Einddatum_Then_Throws_StartdatumLigtNaEinddatum()
    {
        DateOnly? startdatum = _baseDataOnly.AddDays(5);
        DateOnly? einddatum = _baseDataOnly;

        var exception = Assert.Throws<StartdatumLigtNaEinddatum>(() => ErkenningsPeriode.Create(startdatum, einddatum));

        exception.Message.Should().Be(ExceptionMessages.StartdatumIsAfterEinddatum);
    }
}

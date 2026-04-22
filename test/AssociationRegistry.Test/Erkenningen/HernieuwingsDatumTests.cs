namespace AssociationRegistry.Test.Erkenningen;

using AutoFixture;
using Common.AutoFixture;
using DecentraalBeheer.Vereniging.Erkenningen;
using DecentraalBeheer.Vereniging.Erkenningen.Exceptions;
using FluentAssertions;
using Xunit;

public class HernieuwingsdatumTests
{
    private readonly DateOnly _baseDateOnly;

    public HernieuwingsdatumTests()
    {
        var fixture = new Fixture().CustomizeDomain();
        _baseDateOnly = fixture.Create<DateOnly>();
    }

    [Fact]
    public void Given_Empty_Hernieuwingsdatum_Then_Create_Succeeds()
    {
        DateOnly? hernieuwingsdatum = null;
        var erkenningsPeriode = ErkenningsPeriode.Create(_baseDateOnly, _baseDateOnly.AddDays(10));

        var result = Hernieuwingsdatum.Create(hernieuwingsdatum, erkenningsPeriode);

        result.Value.Should().Be(hernieuwingsdatum);
    }

    [Fact]
    public void Given_Hernieuwingsdatum_And_Empty_Startdatum_And_Empty_Einddatum_Then_Create_Succeeds()
    {
        DateOnly? hernieuwingsdatum = _baseDateOnly;
        var erkenningsPeriode = ErkenningsPeriode.Create(null, null);

        var result = Hernieuwingsdatum.Create(hernieuwingsdatum, erkenningsPeriode);

        result.Value.Should().Be(hernieuwingsdatum);
    }

    [Fact]
    public void Given_Hernieuwingsdatum_Equals_Startdatum_Then_Create_Succeeds()
    {
        DateOnly? hernieuwingsdatum = _baseDateOnly;
        var erkenningsPeriode = ErkenningsPeriode.Create(_baseDateOnly, _baseDateOnly.AddDays(10));

        var result = Hernieuwingsdatum.Create(hernieuwingsdatum, erkenningsPeriode);

        result.Value.Should().Be(hernieuwingsdatum);
    }

    [Fact]
    public void Given_Hernieuwingsdatum_Equals_Einddatum_Then_Create_Succeeds()
    {
        DateOnly? hernieuwingsdatum = _baseDateOnly.AddDays(10);
        var erkenningsPeriode = ErkenningsPeriode.Create(_baseDateOnly, _baseDateOnly.AddDays(10));

        var result = Hernieuwingsdatum.Create(hernieuwingsdatum, erkenningsPeriode);

        result.Value.Should().Be(hernieuwingsdatum);
    }

    [Fact]
    public void Given_Hernieuwingsdatum_Between_Startdatum_And_Einddatum_Then_Create_Succeeds()
    {
        DateOnly? hernieuwingsdatum = _baseDateOnly.AddDays(5);
        var erkenningsPeriode = ErkenningsPeriode.Create(_baseDateOnly, _baseDateOnly.AddDays(10));

        var result = Hernieuwingsdatum.Create(hernieuwingsdatum, erkenningsPeriode);

        result.Value.Should().Be(hernieuwingsdatum);
    }

    [Fact]
    public void Given_Hernieuwingsdatum_Before_Startdatum_Then_Throws_HernieuwingsDatumMoetTussenStartEnEindDatumLiggen()
    {
        DateOnly? hernieuwingsdatum = _baseDateOnly.AddDays(-1);
        var erkenningsPeriode = ErkenningsPeriode.Create(_baseDateOnly, _baseDateOnly.AddDays(10));

        Assert.Throws<HernieuwingsDatumMoetTussenStartEnEindDatumLiggen>(() =>
            Hernieuwingsdatum.Create(hernieuwingsdatum, erkenningsPeriode)
        );
    }

    [Fact]
    public void Given_Hernieuwingsdatum_After_Einddatum_Then_Throws_HernieuwingsDatumMoetTussenStartEnEindDatumLiggen()
    {
        DateOnly? hernieuwingsdatum = _baseDateOnly.AddDays(11);
        var erkenningsPeriode = ErkenningsPeriode.Create(_baseDateOnly, _baseDateOnly.AddDays(10));

        Assert.Throws<HernieuwingsDatumMoetTussenStartEnEindDatumLiggen>(() =>
            Hernieuwingsdatum.Create(hernieuwingsdatum, erkenningsPeriode)
        );
    }

    [Fact]
    public void Given_Hernieuwingsdatum_And_Empty_Startdatum_And_Before_Einddatum_Then_Create_Succeeds()
    {
        DateOnly? hernieuwingsdatum = _baseDateOnly;
        var erkenningsPeriode = ErkenningsPeriode.Create(null, _baseDateOnly.AddDays(5));

        var result = Hernieuwingsdatum.Create(hernieuwingsdatum, erkenningsPeriode);

        result.Value.Should().Be(hernieuwingsdatum);
    }

    [Fact]
    public void Given_Hernieuwingsdatum_And_Empty_Startdatum_And_After_Einddatum_Then_Throws_HernieuwingsDatumMoetTussenStartEnEindDatumLiggen()
    {
        DateOnly? hernieuwingsdatum = _baseDateOnly.AddDays(6);
        var erkenningsPeriode = ErkenningsPeriode.Create(null, _baseDateOnly.AddDays(5));

        Assert.Throws<HernieuwingsDatumMoetTussenStartEnEindDatumLiggen>(() =>
            Hernieuwingsdatum.Create(hernieuwingsdatum, erkenningsPeriode)
        );
    }

    [Fact]
    public void Given_Hernieuwingsdatum_And_After_Startdatum_And_Empty_Einddatum_Then_Create_Succeeds()
    {
        DateOnly? hernieuwingsdatum = _baseDateOnly.AddDays(5);
        var erkenningsPeriode = ErkenningsPeriode.Create(_baseDateOnly, null);

        var result = Hernieuwingsdatum.Create(hernieuwingsdatum, erkenningsPeriode);

        result.Value.Should().Be(hernieuwingsdatum);
    }

    [Fact]
    public void Given_Hernieuwingsdatum_And_Before_Startdatum_And_Empty_Einddatum_Then_Throws_HernieuwingsDatumMoetTussenStartEnEindDatumLiggen()
    {
        DateOnly? hernieuwingsdatum = _baseDateOnly.AddDays(-1);
        var erkenningsPeriode = ErkenningsPeriode.Create(_baseDateOnly, null);

        Assert.Throws<HernieuwingsDatumMoetTussenStartEnEindDatumLiggen>(() =>
            Hernieuwingsdatum.Create(hernieuwingsdatum, erkenningsPeriode)
        );
    }
}

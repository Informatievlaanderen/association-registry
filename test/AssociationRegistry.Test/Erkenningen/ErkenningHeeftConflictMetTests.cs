namespace AssociationRegistry.Test.Erkenningen;

using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen;
using AutoFixture;
using Common.AutoFixture;
using FluentAssertions;
using Xunit;

public class ErkenningHeeftConflictMetTests
{
    private readonly DateOnly _baseDateOnly;
    private readonly Fixture _fixture;

    public ErkenningHeeftConflictMetTests()
    {
        _fixture = new Fixture().CustomizeDomain();
        _baseDateOnly = _fixture.Create<DateOnly>();
    }

    [Fact]
    public void Given_Same_Productnummer_And_GeregistreerdDoor_And_Overlapping_Period_Then_HeeftConflictMet_Returns_True()
    {
        var bestaande = _fixture.Create<Erkenning>() with
        {
            ErkenningsPeriode = ErkenningsPeriode.Create(_baseDateOnly, _baseDateOnly.AddDays(10)),
        };

        var toeTeVoegen = bestaande with
        {
            ErkenningsPeriode = ErkenningsPeriode.Create(_baseDateOnly.AddDays(5), _baseDateOnly.AddDays(15)),
        };

        var result = bestaande.HeeftConflictMet(toeTeVoegen);

        result.Should().BeTrue();
    }

    [Fact]
    public void Given_Same_Productnummer_And_GeregistreerdDoor_And_Non_Overlapping_Period_Then_HeeftConflictMet_Returns_False()
    {
        var bestaande = _fixture.Create<Erkenning>() with
        {
            ErkenningsPeriode = ErkenningsPeriode.Create(_baseDateOnly, _baseDateOnly.AddDays(10)),
        };

        var toeTeVoegen = bestaande with
        {
            ErkenningsPeriode = ErkenningsPeriode.Create(_baseDateOnly.AddDays(11), _baseDateOnly.AddDays(20)),
        };

        var result = bestaande.HeeftConflictMet(toeTeVoegen);

        result.Should().BeFalse();
    }

    [Fact]
    public void Given_Different_Productnummer_Then_HeeftConflictMet_Returns_False()
    {
        var productNummer1 = "product-1";
        var productNummer2 = "product-2";

        var zelfdeNaam = "naam";

        var bestaande = _fixture.Create<Erkenning>() with
        {
            IpdcProduct = new IpdcProduct { Nummer = productNummer1, Naam = zelfdeNaam },
            ErkenningsPeriode = ErkenningsPeriode.Create(_baseDateOnly, _baseDateOnly.AddDays(10)),
        };

        var toeTeVoegen = bestaande with
        {
            IpdcProduct = new IpdcProduct { Nummer = productNummer2, Naam = zelfdeNaam },
            ErkenningsPeriode = ErkenningsPeriode.Create(_baseDateOnly.AddDays(5), _baseDateOnly.AddDays(15)),
        };

        var result = bestaande.HeeftConflictMet(toeTeVoegen);

        result.Should().BeFalse();
    }

    [Fact]
    public void Given_Different_GeregistreerdDoor_Then_HeeftConflictMet_Returns_False()
    {
        var ovoCode1 = "ovo-1";
        var ovoCode2 = "ovo-2";

        var zelfdeNaam = "naam";

        var bestaande = _fixture.Create<Erkenning>() with
        {
            GeregistreerdDoor = new GegevensInitiator { OvoCode = ovoCode1, Naam = zelfdeNaam },
            ErkenningsPeriode = ErkenningsPeriode.Create(_baseDateOnly, _baseDateOnly.AddDays(10)),
        };

        var toeTeVoegen = bestaande with
        {
            GeregistreerdDoor = new GegevensInitiator { OvoCode = ovoCode2, Naam = zelfdeNaam },
            ErkenningsPeriode = ErkenningsPeriode.Create(_baseDateOnly.AddDays(5), _baseDateOnly.AddDays(15)),
        };

        var result = bestaande.HeeftConflictMet(toeTeVoegen);

        result.Should().BeFalse();
    }

    [Fact]
    public void Given_Touching_Boundary_Then_HeeftConflictMet_Returns_True()
    {
        var bestaande = _fixture.Create<Erkenning>() with
        {
            ErkenningsPeriode = ErkenningsPeriode.Create(_baseDateOnly, _baseDateOnly.AddDays(10)),
        };

        var toeTeVoegen = bestaande with
        {
            ErkenningsPeriode = ErkenningsPeriode.Create(_baseDateOnly.AddDays(10), _baseDateOnly.AddDays(20)),
        };

        var result = bestaande.HeeftConflictMet(toeTeVoegen);

        result.Should().BeTrue();
    }

    [Fact]
    public void Given_Null_Startdatum_Then_HeeftConflictMet_Returns_True()
    {
        var bestaande = _fixture.Create<Erkenning>() with
        {
            ErkenningsPeriode = ErkenningsPeriode.Create(null, _baseDateOnly.AddDays(10)),
        };

        var toeTeVoegen = bestaande with
        {
            ErkenningsPeriode = ErkenningsPeriode.Create(_baseDateOnly.AddDays(-20), _baseDateOnly.AddDays(5)),
        };

        var result = bestaande.HeeftConflictMet(toeTeVoegen);

        result.Should().BeTrue();
    }

    [Fact]
    public void Given_Null_Einddatum_Then_HeeftConflictMet_Returns_True()
    {
        var bestaande = _fixture.Create<Erkenning>() with
        {
            ErkenningsPeriode = ErkenningsPeriode.Create(_baseDateOnly, null),
        };

        var toeTeVoegen = bestaande with
        {
            ErkenningsPeriode = ErkenningsPeriode.Create(_baseDateOnly.AddDays(100), _baseDateOnly.AddDays(120)),
        };

        var result = bestaande.HeeftConflictMet(toeTeVoegen);

        result.Should().BeTrue();
    }

    [Fact]
    public void Given_Both_Open_Periods_Then_HeeftConflictMet_Returns_True()
    {
        var bestaande = _fixture.Create<Erkenning>() with { ErkenningsPeriode = ErkenningsPeriode.Create(null, null) };

        var toeTeVoegen = bestaande with { ErkenningsPeriode = ErkenningsPeriode.Create(null, null) };

        var result = bestaande.HeeftConflictMet(toeTeVoegen);

        result.Should().BeTrue();
    }
}

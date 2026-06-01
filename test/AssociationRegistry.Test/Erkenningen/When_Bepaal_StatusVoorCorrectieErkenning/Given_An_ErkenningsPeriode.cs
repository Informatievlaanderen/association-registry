namespace AssociationRegistry.Test.Erkenningen.When_Bepaal_StatusVoorCorrectieErkenning;

using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen;
using AutoFixture;
using Common.AutoFixture;
using FluentAssertions;
using Xunit;

public class Given_An_ErkenningsPeriode
{
    private static DateOnly Now => DateOnly.FromDateTime(DateTime.Now);

    [Fact]
    public void Given_Huidige_Status_Is_Geschorst_Then_Returns_Geschorst()
    {
        var fixture = new Fixture().CustomizeDomain();

        var status = ErkenningStatus.BepaalVoorWijziging(
            ErkenningStatus.Geschorst,
            ErkenningsPeriode.Hydrate(fixture.Create<DateOnly>(), fixture.Create<DateOnly>()),
            Now
        );

        status.Should().Be(ErkenningStatus.Geschorst);
    }

    [Fact]
    public void Given_Empty_Startdatum_And_Empty_EindDatum_Then_Status_Actief()
    {
        var fixture = new Fixture().CustomizeDomain();

        DateOnly? startDatum = null;
        DateOnly? eindDatum = null;
        var erkenningsPeriode = ErkenningsPeriode.Create(startDatum, eindDatum);

        var status = ErkenningStatus.BepaalVoorWijziging(fixture.Create<ErkenningStatus>(), erkenningsPeriode, Now);

        status.Should().Be(ErkenningStatus.Actief);
    }

    [Fact]
    public void Given_Past_Startdatum_And_Empty_EindDatum_Then_Status_Actief()
    {
        var fixture = new Fixture().CustomizeDomain();

        DateOnly? startDatum = Now.AddDays(-1);
        DateOnly? eindDatum = null;
        var erkenningsPeriode = ErkenningsPeriode.Create(startDatum, eindDatum);

        var status = ErkenningStatus.BepaalVoorWijziging(fixture.Create<ErkenningStatus>(), erkenningsPeriode, Now);

        status.Should().Be(ErkenningStatus.Actief);
    }

    [Fact]
    public void Given_Startdatum_Today_And_Empty_EindDatum_Then_Status_Actief()
    {
        var fixture = new Fixture().CustomizeDomain();

        DateOnly? startDatum = Now;
        DateOnly? eindDatum = null;
        var erkenningsPeriode = ErkenningsPeriode.Create(startDatum, eindDatum);

        var status = ErkenningStatus.BepaalVoorWijziging(fixture.Create<ErkenningStatus>(), erkenningsPeriode, Now);

        status.Should().Be(ErkenningStatus.Actief);
    }

    [Fact]
    public void Given_Future_Startdatum_And_Empty_EindDatum_Then_Status_Actief()
    {
        var fixture = new Fixture().CustomizeDomain();

        DateOnly? startDatum = Now.AddDays(5);
        DateOnly? eindDatum = null;
        var erkenningsPeriode = ErkenningsPeriode.Create(startDatum, eindDatum);

        var status = ErkenningStatus.BepaalVoorWijziging(fixture.Create<ErkenningStatus>(), erkenningsPeriode, Now);

        status.Should().Be(ErkenningStatus.InAanvraag);
    }

    [Fact]
    public void Given_Empty_Startdatum_And_Past_EindDatum_Then_Status_Verlopen()
    {
        var fixture = new Fixture().CustomizeDomain();

        DateOnly? startDatum = null;
        DateOnly? eindDatum = Now.AddDays(-5);
        var erkenningsPeriode = ErkenningsPeriode.Create(startDatum, eindDatum);

        var status = ErkenningStatus.BepaalVoorWijziging(fixture.Create<ErkenningStatus>(), erkenningsPeriode, Now);

        status.Should().Be(ErkenningStatus.Verlopen);
    }

    [Fact]
    public void Given_Past_Startdatum_And_Past_EindDatum_Then_Status_Verlopen()
    {
        var fixture = new Fixture().CustomizeDomain();

        DateOnly? startDatum = Now.AddDays(-10);
        DateOnly? eindDatum = Now.AddDays(-5);
        var erkenningsPeriode = ErkenningsPeriode.Create(startDatum, eindDatum);

        var status = ErkenningStatus.BepaalVoorWijziging(fixture.Create<ErkenningStatus>(), erkenningsPeriode, Now);

        status.Should().Be(ErkenningStatus.Verlopen);
    }

    [Fact]
    public void Given_Empty_Startdatum_And_EindDatum_Today_Then_Status_Actief()
    {
        var fixture = new Fixture().CustomizeDomain();

        DateOnly? startDatum = null;
        DateOnly? eindDatum = Now;
        var erkenningsPeriode = ErkenningsPeriode.Create(startDatum, eindDatum);

        var status = ErkenningStatus.BepaalVoorWijziging(fixture.Create<ErkenningStatus>(), erkenningsPeriode, Now);

        status.Should().Be(ErkenningStatus.Actief);
    }

    [Fact]
    public void Given_Past_Startdatum_And_EindDatum_Today_Then_Status_Actief()
    {
        var fixture = new Fixture().CustomizeDomain();

        DateOnly? startDatum = Now.AddDays(-10);
        DateOnly? eindDatum = Now;
        var erkenningsPeriode = ErkenningsPeriode.Create(startDatum, eindDatum);

        var status = ErkenningStatus.BepaalVoorWijziging(fixture.Create<ErkenningStatus>(), erkenningsPeriode, Now);

        status.Should().Be(ErkenningStatus.Actief);
    }

    [Fact]
    public void Given_Startdatum_Today_And_EindDatum_Today_Then_Status_Actief()
    {
        var fixture = new Fixture().CustomizeDomain();

        DateOnly? startDatum = Now;
        DateOnly? eindDatum = Now;
        var erkenningsPeriode = ErkenningsPeriode.Create(startDatum, eindDatum);

        var status = ErkenningStatus.BepaalVoorWijziging(fixture.Create<ErkenningStatus>(), erkenningsPeriode, Now);

        status.Should().Be(ErkenningStatus.Actief);
    }

    [Fact]
    public void Given_Empty_Startdatum_And_EindDatum_Future_Then_Status_Actief()
    {
        var fixture = new Fixture().CustomizeDomain();

        DateOnly? startDatum = null;
        DateOnly? eindDatum = Now.AddDays(5);
        var erkenningsPeriode = ErkenningsPeriode.Create(startDatum, eindDatum);

        var status = ErkenningStatus.BepaalVoorWijziging(fixture.Create<ErkenningStatus>(), erkenningsPeriode, Now);

        status.Should().Be(ErkenningStatus.Actief);
    }

    [Fact]
    public void Given_Past_Startdatum_And_EindDatum_Future_Then_Status_Actief()
    {
        var fixture = new Fixture().CustomizeDomain();

        DateOnly? startDatum = Now.AddDays(-5);
        DateOnly? eindDatum = Now.AddDays(5);
        var erkenningsPeriode = ErkenningsPeriode.Create(startDatum, eindDatum);

        var status = ErkenningStatus.BepaalVoorWijziging(fixture.Create<ErkenningStatus>(), erkenningsPeriode, Now);

        status.Should().Be(ErkenningStatus.Actief);
    }

    [Fact]
    public void Given_Startdatum_Today_And_EindDatum_Future_Then_Status_Actief()
    {
        var fixture = new Fixture().CustomizeDomain();

        DateOnly? startDatum = Now;
        DateOnly? eindDatum = Now.AddDays(5);
        var erkenningsPeriode = ErkenningsPeriode.Create(startDatum, eindDatum);

        var status = ErkenningStatus.BepaalVoorWijziging(fixture.Create<ErkenningStatus>(), erkenningsPeriode, Now);

        status.Should().Be(ErkenningStatus.Actief);
    }

    [Fact]
    public void Given_Future_Startdatum_And_EindDatum_Future_Then_Status_InAanvraag()
    {
        var fixture = new Fixture().CustomizeDomain();

        DateOnly? startDatum = Now.AddDays(3);
        DateOnly? eindDatum = Now.AddDays(5);
        var erkenningsPeriode = ErkenningsPeriode.Create(startDatum, eindDatum);

        var status = ErkenningStatus.BepaalVoorWijziging(fixture.Create<ErkenningStatus>(), erkenningsPeriode, Now);

        status.Should().Be(ErkenningStatus.InAanvraag);
    }
}

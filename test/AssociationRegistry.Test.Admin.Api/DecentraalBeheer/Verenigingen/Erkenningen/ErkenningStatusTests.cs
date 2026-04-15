namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen;

using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen;
using FluentAssertions;
using Xunit;

public class ErkenningStatusTests
{
    private static DateOnly Now => DateOnly.FromDateTime(DateTime.Now);

    [Fact]
    public void Given_Empty_Startdatum_And_Empty_EindDatum_Then_Status_Actief()
    {
        DateOnly? startDatum = null;
        DateOnly? eindDatum = null;

        var status = ErkenningStatus.Calculate(startDatum, eindDatum);

        status.Should().Be("Actief");
    }


    [Fact]
    public void Given_Past_Startdatum_And_Empty_EindDatum_Then_Status_Actief()
    {
        DateOnly? startDatum = Now.AddDays(-1);
        DateOnly? eindDatum = null;

        var status = ErkenningStatus.Calculate(startDatum, eindDatum);

        status.Should().Be("Actief");
    }

    [Fact]
    public void Given_Startdatum_Today_And_Empty_EindDatum_Then_Status_Actief()
    {
        DateOnly? startDatum = Now;
        DateOnly? eindDatum = null;

        var status = ErkenningStatus.Calculate(startDatum, eindDatum);

        status.Should().Be("Actief");
    }


    [Fact]
    public void Given_Future_Startdatum_And_Empty_EindDatum_Then_Status_Actief()
    {
        DateOnly? startDatum = Now.AddDays(5);
        DateOnly? eindDatum = null;

        var status = ErkenningStatus.Calculate(startDatum, eindDatum);

        status.Should().Be("InAanvraag");
    }


    [Fact]
    public void Given_Empty_Startdatum_And_Past_EindDatum_Then_Status_Verlopen()
    {
        DateOnly? startDatum = null;
        DateOnly? eindDatum = Now.AddDays(-5);

        var status = ErkenningStatus.Calculate(startDatum, eindDatum);

        status.Should().Be("Verlopen");
    }

    [Fact]
    public void Given_Past_Startdatum_And_Past_EindDatum_Then_Status_Verlopen()
    {
        DateOnly? startDatum = Now.AddDays(-10);
        DateOnly? eindDatum = Now.AddDays(-5);

        var status = ErkenningStatus.Calculate(startDatum, eindDatum);

        status.Should().Be("Verlopen");
    }


    [Fact]
    public void Given_Empty_Startdatum_And_EindDatum_Today_Then_Status_Actief()
    {
        DateOnly? startDatum = null;
        DateOnly? eindDatum = Now;

        var status = ErkenningStatus.Calculate(startDatum, eindDatum);

        status.Should().Be("Actief");
    }


    [Fact]
    public void Given_Past_Startdatum_And_EindDatum_Today_Then_Status_Actief()
    {
        DateOnly? startDatum = Now.AddDays(-10);
        DateOnly? eindDatum = Now;

        var status = ErkenningStatus.Calculate(startDatum, eindDatum);

        status.Should().Be("Actief");
    }

    [Fact]
    public void Given_Startdatum_Today_And_EindDatum_Today_Then_Status_Actief()
    {
        DateOnly? startDatum = Now;
        DateOnly? eindDatum = Now;

        var status = ErkenningStatus.Calculate(startDatum, eindDatum);

        status.Should().Be("Actief");
    }

    [Fact]
    public void Given_Empty_Startdatum_And_EindDatum_Future_Then_Status_Actief()
    {
        DateOnly? startDatum = null;
        DateOnly? eindDatum = Now.AddDays(5);

        var status = ErkenningStatus.Calculate(startDatum, eindDatum);

        status.Should().Be("Actief");
    }

    [Fact]
    public void Given_Past_Startdatum_And_EindDatum_Future_Then_Status_Actief()
    {
        DateOnly? startDatum = Now.AddDays(-5);
        DateOnly? eindDatum = Now.AddDays(5);

        var status = ErkenningStatus.Calculate(startDatum, eindDatum);

        status.Should().Be("Actief");
    }

    [Fact]
    public void Given_Startdatum_Today_And_EindDatum_Future_Then_Status_Actief()
    {
        DateOnly? startDatum = Now;
        DateOnly? eindDatum = Now.AddDays(5);

        var status = ErkenningStatus.Calculate(startDatum, eindDatum);

        status.Should().Be("Actief");
    }

    [Fact]
    public void Given_Future_Startdatum_And_EindDatum_Future_Then_Status_InAanvraag()
    {
        DateOnly? startDatum = Now.AddDays(3);
        DateOnly? eindDatum = Now.AddDays(5);

        var status = ErkenningStatus.Calculate(startDatum, eindDatum);

        status.Should().Be("InAanvraag");
    }
}

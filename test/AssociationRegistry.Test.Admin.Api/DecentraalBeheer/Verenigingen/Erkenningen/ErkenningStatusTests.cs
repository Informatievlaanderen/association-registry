namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen;

using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen;
using FluentAssertions;
using Xunit;

public class ErkenningStatusTests
{
    private static DateOnly Now => DateOnly.FromDateTime(DateTime.Now);

    [Fact]
    public void Given_Startdatum_After_Now_Then_InAanvraag()
    {
        var startDatum = Now.AddDays(1);
        var eindDatum = Now.AddDays(30);

        var status = ErkenningStatus.Create(startDatum, eindDatum);

        status.Value.Should().Be("InAanvraag");
    }

    [Fact]
    public void Given_Startdatum_Before_Now_And_EindDatum_After_Now_Then_Actief()
    {
        var startDatum = Now.AddDays(-1);
        var eindDatum = Now.AddDays(30);

        var status = ErkenningStatus.Create(startDatum, eindDatum);

        status.Value.Should().Be("Actief");
    }

    [Fact]
    public void Given_Startdatum_Equal_Now_And_EindDatum_After_Now_Then_Actief()
    {
        var startDatum = Now;
        var eindDatum = Now.AddDays(30);

        var status = ErkenningStatus.Create(startDatum, eindDatum);

        status.Value.Should().Be("Actief");
    }

    [Fact]
    public void Given_Startdatum_Before_Now_And_EindDatum_Equal_Now_Then_Actief()
    {
        var startDatum = Now.AddDays(-30);
        var eindDatum = Now;

        var status = ErkenningStatus.Create(startDatum, eindDatum);

        status.Value.Should().Be("Actief");
    }

    [Fact]
    public void Given_Startdatum_Before_Now_And_EindDatum_Before_Now_Then_Verlopen()
    {
        var startDatum = Now.AddDays(-30);
        var eindDatum = Now.AddDays(-1);

        var status = ErkenningStatus.Create(startDatum, eindDatum);

        status.Value.Should().Be("Verlopen");
    }

}
